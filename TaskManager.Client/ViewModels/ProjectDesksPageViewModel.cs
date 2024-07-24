using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Pages;
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
        public DelegateCommand<object> OpenDeskTasksPageCommand {  get; private set; }
        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private Window _ownerWindow { get; set; }
        private ProjectModel _project { get; set; }
        private MainWindowViewModel _mainWindowViewModel { get; set; }
        private DesksRequestService _desksRequestService { get; set; }
        private UsersRequestService _usersRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }
        private DesksViewService _deskViewService { get; set; }

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
            set
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
            _mainWindowViewModel = mainWindowVM;
            _ownerWindow = mainWindowVM.CurrentWindow;

            _desksRequestService = new DesksRequestService();
            _usersRequestService = new UsersRequestService();
            _commonViewService = new CommonViewService();
            _deskViewService = new DesksViewService(token, _desksRequestService, _commonViewService);

            InitializeCurrentUserAsync();
            LoadProjectDesksAsync();

            OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
            OpenUpdateDeskCommand = new DelegateCommand<object>(OpenUpdateDeskAsync);
            CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDeskAsync);
            DeleteDeskCommand = new DelegateCommand(DeleteDeskAsync);
            SelectImageForDeskCommand = new DelegateCommand(() =>
            {
                var selectedDesk = SelectedDesk;
                _deskViewService.SelectImageForDesk(ref selectedDesk);
                SelectedDesk = selectedDesk;
            });
            AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
            RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
            OpenDeskTasksPageCommand = new DelegateCommand<object>(OpenDeskTasksPageAsync);
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
        private async Task UpdatePageAsync()
        {
            await LoadProjectDesksAsync();
            SelectedDesk = null;
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
            SelectedDesk = await _deskViewService.GetDeskClientByIdAsync(deskId);

            if (CurrentUser.Id != SelectedDesk.Model.Id)
            {
                _commonViewService.ShowMessage("You are not admin!");
                return;
            }

            TypeActionWithDesk = ClientAction.Update;

            ColumnsForNewDesk =  new ObservableCollection<ColumnBindingHelper>(SelectedDesk.Model.Columns.Select(c => new ColumnBindingHelper(c)));

            _deskViewService.OpenViewDeskInfo(deskId, this, _ownerWindow);
        }
        private async void CreateOrUpdateDeskAsync()
        {
            switch (TypeActionWithDesk)
            {
                case ClientAction.Create:
                    await CreateDeskAsync();
                    break;
                case ClientAction.Update:
                    SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
                    await _deskViewService.UpdateDeskAsync(SelectedDesk.Model);
                    break;
            }
            await UpdatePageAsync();
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private async Task CreateDeskAsync()
        {
            SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
            SelectedDesk.Model.ProjectId = _project.Id;

            var resultAction = await _desksRequestService.CreateDesk(_token, SelectedDesk.Model);
            _commonViewService.ShowActionResult(resultAction, "New desk created successfully");
        }
        private async void DeleteDeskAsync()
        {
            await _deskViewService.DeleteDeskAsync(SelectedDesk.Model.Id);
            UpdatePageAsync();
        }
        private void AddNewColumnItem() => ColumnsForNewDesk.Add(new ColumnBindingHelper("Column"));
        private void RemoveColumnItem(object item)
        {
            var itemToRemove = (ColumnBindingHelper)item;

            ColumnsForNewDesk.Remove(itemToRemove);
        }

        private async void OpenDeskTasksPageAsync(object deskId)
        {
            SelectedDesk = await _deskViewService.GetDeskClientByIdAsync((int)deskId);
            var page = new DeskTasksPage();
            var context = new DeskTasksPageViewModel(_token, SelectedDesk.Model, page);
            _mainWindowViewModel.OpenPage(page, $"Tasks of {SelectedDesk.Model.Name}", context);
        }
        #endregion
    }
}
