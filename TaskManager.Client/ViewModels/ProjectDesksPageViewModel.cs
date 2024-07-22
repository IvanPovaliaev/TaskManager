using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectDesksPageViewModel : BindableBase
    {
        #region COMMAND
        public DelegateCommand OpenNewDeskCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
        public DelegateCommand DeleteDeskCommand { get; private set; }
        public DelegateCommand SelectImageForDeskCommand { get; private set; }
        public DelegateCommand AddNewColumnItemCommand { get; private set; } 
        public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }
        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private Window _ownerWindow { get; set; }
        private ProjectModel _project { get; set; }
        private DesksRequestService _desksRequestService { get; set; }
        private UsersRequestService _usersRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }

        private UserModel _currentUser;
        public UserModel CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                RaisePropertyChanged(nameof(CurrentUser));
            }
        }

        private List<ModelClient<DeskModel>> _projectDesks = [];
        public List<ModelClient<DeskModel>> ProjectDesks
        {
            get => _projectDesks;
            set
            {
                _projectDesks = value;
                RaisePropertyChanged(nameof(ProjectDesks));
            }
        }

        private ClientAction _typeActionWithDesk;
        public ClientAction TypeActionWithDesk
        {
            get => _typeActionWithDesk;
            set
            {
                _typeActionWithDesk = value;
                RaisePropertyChanged(nameof(TypeActionWithDesk));
            }
        }

        private ModelClient<DeskModel> _selectedDesk;
        public ModelClient<DeskModel> SelectedDesk
        {
            get => _selectedDesk;
            private set
            {
                _selectedDesk = value;
                RaisePropertyChanged(nameof(SelectedDesk));
            }
        }

        private ObservableCollection<ColumnBindingHelper> _columnsForNewDesk = new ObservableCollection<ColumnBindingHelper>
        {
            new ColumnBindingHelper("New"),
            new ColumnBindingHelper("In progress"),
            new ColumnBindingHelper("In review"),
            new ColumnBindingHelper("Completed")
        };
        public ObservableCollection<ColumnBindingHelper> ColumnsForNewDesk
        {
            get => _columnsForNewDesk;
            set
            {
                _columnsForNewDesk = value;
                RaisePropertyChanged(nameof(ColumnsForNewDesk));
            }
        }
        #endregion

        public ProjectDesksPageViewModel(AuthToken token, ProjectModel project, MainWindowViewModel mainWindowVM)
        {
            _token = token;
            _project = project;
            _ownerWindow = mainWindowVM.CurrentWindow;

            _desksRequestService = new DesksRequestService();
            _usersRequestService = new UsersRequestService();
            _commonViewService = new CommonViewService();

            InitializeCurrentUserAsync();
            LoadProjectDesksAsync();

            OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
            OpenUpdateDeskCommand = new DelegateCommand<object>(OpenUpdateDeskAsync);
            CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDeskAsync);
            DeleteDeskCommand = new DelegateCommand(DeleteDeskAsync);
            SelectImageForDeskCommand = new DelegateCommand(SelectImageForDesk);
            AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
            RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
        }

        #region METHODS
        private async Task InitializeCurrentUserAsync()
        {
            CurrentUser = await _usersRequestService.GetCurrentUser(_token);
        }
        private async Task LoadProjectDesksAsync()
        {
            var desks = await _desksRequestService.GetDesksByProject(_token, _project.Id);

            ProjectDesks = desks?.Select(desk => new ModelClient<DeskModel>(desk)).ToList();
        }
        private async Task UpdatePage()
        {
            await LoadProjectDesksAsync();
            SelectedDesk = null;
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private void OpenNewDesk()
        {
            SelectedDesk = new ModelClient<DeskModel>(new DeskModel());

            TypeActionWithDesk = ClientAction.Create;
            var window = new CreateOrUpdateDeskWindow();
            window.Owner = _ownerWindow;

            _commonViewService.OpenWindow(window, this);
        }
        private async void OpenUpdateDeskAsync(object deskId)
        {
            SelectedDesk = await GetDeskClientByIdAsync(deskId);

            TypeActionWithDesk = ClientAction.Update;
            var window = new CreateOrUpdateDeskWindow();
            window.Owner = _ownerWindow;

            _commonViewService.OpenWindow(window, this);
        }
        private async Task<ModelClient<DeskModel>> GetDeskClientByIdAsync(object deskId)
        {
            try
            {
                var selectedDesk = await _desksRequestService.GetDeskById(_token, (int)deskId);
                return new ModelClient<DeskModel>(selectedDesk);
            }
            catch (FormatException ex)
            {
                return new ModelClient<DeskModel>(null);
            }
        }
        private async void CreateOrUpdateDeskAsync()
        {
            switch (TypeActionWithDesk)
            {
                case ClientAction.Create:
                    await CreateDeskAsync();
                    break;
                case ClientAction.Update:
                    await UpdateDeskAsync();
                    break;
            }
            await UpdatePage();
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private async Task CreateDeskAsync()
        {
            SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
            SelectedDesk.Model.ProjectId = _project.Id;

            var resultAction = await _desksRequestService.CreateDesk(_token, SelectedDesk.Model);
            _commonViewService.ShowActionResult(resultAction, "New desk created successfully");
        }
        private async Task UpdateDeskAsync()
        {
            var resultAction = await _desksRequestService.UpdateDesk(_token, SelectedDesk.Model);
            _commonViewService.ShowActionResult(resultAction, "Desk updated successfully");       
        }
        private async void DeleteDeskAsync()
        {
            var resultAction = await _desksRequestService.DeleteDesk(_token, SelectedDesk.Model.Id);
            UpdatePage();
            _commonViewService.CurrentOpenWindow?.Close();
            _commonViewService.ShowActionResult(resultAction, "Desk deleted successfully");
        }
        private void SelectImageForDesk()
        {
            _commonViewService.SetImageForObject(SelectedDesk.Model);
            SelectedDesk = new ModelClient<DeskModel>(SelectedDesk.Model);
        }
        private void AddNewColumnItem() => ColumnsForNewDesk.Add(new ColumnBindingHelper("Column"));
        private void RemoveColumnItem(object item)
        {
            var itemToRemove = (ColumnBindingHelper)item;

            ColumnsForNewDesk.Remove(itemToRemove);
        }
        #endregion
    }
}
