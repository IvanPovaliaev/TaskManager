using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Components;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class DeskTasksPageViewModel : BindableBase
    {
        #region COMMANDS
        public DelegateCommand OpenNewTaskCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateTaskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateTaskCommand { get; private set; }
        public DelegateCommand DeleteTaskCommand { get; private set; }
        public DelegateCommand SelectFileForTaskCommand { get; private set; }
        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private DeskModel _desk { get; set; }
        private Window _ownerWindow { get; set; }
        private DeskTasksPage _page { get; set; }
        private UsersRequestService _usersRequestService { get; set; }
        private TasksRequestService _tasksRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }

        private Dictionary<string, List<TaskClient>> _taskByColumns;
        public Dictionary<string, List<TaskClient>> TasksByColumns
        {
            get =>_taskByColumns;
            set
            {
                _taskByColumns = value;
                RaisePropertyChanged(nameof(TasksByColumns));
            }
        }

        private TaskClient _selectedTask;
        public TaskClient SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                RaisePropertyChanged(nameof(SelectedTask));
            }
        }

        private ClientAction _typeActionWithTask;
        public ClientAction TypeActionWithTask
        {
            get => _typeActionWithTask;
            set
            {
                _typeActionWithTask = value;
                RaisePropertyChanged(nameof(TypeActionWithTask));
            }
        }

        #endregion

        public DeskTasksPageViewModel(AuthToken token, DeskModel desk, DeskTasksPage page, MainWindowViewModel? mainWindowVM = null)
        {
            _token = token;
            _desk = desk;
            _page = page;
            _ownerWindow = mainWindowVM?.CurrentWindow;

            _usersRequestService = new UsersRequestService();
            _tasksRequestService = new TasksRequestService();
            _commonViewService = new CommonViewService();

            OpenNewTaskCommand = new DelegateCommand(OpenNewTask);
            OpenUpdateTaskCommand = new DelegateCommand<object>(OpenUpdateTask);
            CreateOrUpdateTaskCommand = new DelegateCommand(CreateOrUpdateTaskAsync);
            DeleteTaskCommand = new DelegateCommand(DeleteTaskAsync);
            SelectFileForTaskCommand = new DelegateCommand(SelectFileForTask);

            page.Loaded += DeskTasksPage_LoadedAsync;            
        }

        #region EVENTS
        private async void DeskTasksPage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await GetTaskByColumns(_desk.Id);
            var grid = await CreateTasksGrid();
            _page.TasksGrid.Children.Add(grid);
        }
        #endregion

        #region METHODS
        private async Task GetTaskByColumns(int deskId)
        {
            var tasksByColumns = new Dictionary<string, List<TaskClient>>();

            var allTasks = await _tasksRequestService.GetTasksByDesk(_token, deskId);

            foreach (var column in _desk.Columns)
            {
                tasksByColumns.Add(column, allTasks.Where(t => t.Column == column)
                                                    .Select(t => new TaskClient(t))
                                                    .ToList());
            }

            TasksByColumns = tasksByColumns;
        }
        private async Task<Grid> CreateTasksGrid()
        {
            var resource = new ResourceDictionary();
            resource.Source = new Uri("./Resources/Styles/MainStyle.xaml", UriKind.Relative);

            var grid = new Grid();
            var row0 = new RowDefinition();
            row0.Height = new GridLength(30);

            var row1 = new RowDefinition();

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);

            var columnCount = 0;

            foreach (var column in TasksByColumns)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                //header
                var header = new TextBlock();
                header.Text = column.Key;
                header.Style = (Style)resource["headerTBlock"];

                Grid.SetRow(header, 0);
                Grid.SetColumn(header, columnCount);

                grid.Children.Add(header);

                //column

                var columnControl = new ItemsControl();
                Grid.SetRow(columnControl, 1);
                Grid.SetColumn(columnControl, columnCount);

                var user = await _usersRequestService.GetCurrentUser(_token);
                var tasksViews = new List<TaskControl>();

                foreach (var task in column.Value)
                {
                    var taskControl = new TaskControl(task);

                    //Create button for Admin and Creator
                    if (task.Model.CreatorId == user.Id || user.Role == UserRole.Admin)
                    {
                        var editBtn = new Button();
                        editBtn.Content = "Edit";
                        editBtn.Style = (Style)resource["commonBtn"];
                        editBtn.Command = OpenUpdateTaskCommand;
                        editBtn.CommandParameter = task.Model;

                        var lastRowNumb = taskControl.TaskGrid.RowDefinitions.Count() - 1;  

                        Grid.SetRow(editBtn, lastRowNumb);
                        Grid.SetColumn(editBtn, 0);

                        taskControl.TaskGrid.Children.Add(editBtn);
                    }
                    
                    tasksViews.Add(taskControl);                    
                }

                columnControl.ItemsSource = tasksViews;
                grid.Children.Add(columnControl);

                columnCount++;
            }

            return grid;
        }
        private async void CreateOrUpdateTaskAsync()
        {
            switch (TypeActionWithTask)
            {
                case ClientAction.Create:                    
                    await CreateTaskAsync();
                    break;
                case ClientAction.Update:
                    await UpdateTaskAsync();
                    break;
            }
            await UpdatePageAsync();
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private async Task UpdatePageAsync()
        {
            SelectedTask = null;
            DeskTasksPage_LoadedAsync(this, new RoutedEventArgs());
        }       
        private void OpenNewTask()
        {
            SelectedTask = new TaskClient(new TaskModel());
            TypeActionWithTask = ClientAction.Create;

            var wnd = new CreateOrUpdateTaskWindow();
            wnd.Owner = _ownerWindow;
            _commonViewService.OpenWindow(wnd, this);
        }
        private void OpenUpdateTask(object taskModel)
        {
            SelectedTask = new TaskClient((TaskModel)taskModel);
            TypeActionWithTask = ClientAction.Update;

            var wnd = new CreateOrUpdateTaskWindow();
            wnd.Owner = _ownerWindow;
            _commonViewService.OpenWindow(wnd, this);
        }
        private async Task CreateTaskAsync()
        {
            SelectedTask.Model.DeskId = _desk.Id;
            SelectedTask.Model.Column = _desk.Columns.FirstOrDefault();

            var resultAction = await _tasksRequestService.CreateTask(_token, SelectedTask.Model);
            _commonViewService.ShowActionResult(resultAction, "New task created successfully");
        }
        public async Task UpdateTaskAsync()
        {
            var resultAction = await _tasksRequestService.UpdateTask(_token, SelectedTask.Model);
            _commonViewService.ShowActionResult(resultAction, "Task updated successfully");
        }
        private async void DeleteTaskAsync()
        {
            await _tasksRequestService.DeleteTask(_token, SelectedTask.Model.Id);
            UpdatePageAsync();
        }
        private void SelectFileForTask()
        {
            _commonViewService.SetFileForTask(SelectedTask.Model);
            SelectedTask = new TaskClient(SelectedTask.Model);
        }
        #endregion
    }
}
