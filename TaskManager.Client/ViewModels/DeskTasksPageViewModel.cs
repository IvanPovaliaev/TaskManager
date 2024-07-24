using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.Components;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class DeskTasksPageViewModel : BindableBase
    {
        #region PROPERTIES
        private AuthToken _token { get; set; }
        private DeskModel _desk { get; set; }
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

        #endregion

        public DeskTasksPageViewModel(AuthToken token, DeskModel desk, DeskTasksPage page)
        {
            _token = token;
            _desk = desk;
            _page = page;

            _usersRequestService = new UsersRequestService();
            _tasksRequestService = new TasksRequestService();
            _commonViewService = new CommonViewService();

            page.Loaded += DeskTasksPage_LoadedAsync;            
        }

        #region EVENTS
        private async void DeskTasksPage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await GetTaskByColumns(_desk.Id);
            _page.TasksGrid.Children.Add(CreateTasksGrid());
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

        private Grid CreateTasksGrid()
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

                var tasksViews = new List<TaskControl>();

                foreach (var task in column.Value)
                {
                    tasksViews.Add(new TaskControl(task));
                }

                columnControl.ItemsSource = tasksViews;
                grid.Children.Add(columnControl);

                columnCount++;
            }

            return grid;
        }
        #endregion
    }
}
