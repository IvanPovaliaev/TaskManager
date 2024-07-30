using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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
    public class ProjectsPageViewModel : BindableBase
    {
        #region COMMANDS
        public DelegateCommand OpenNewProjectCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateProjectCommand { get; private set; }
        public DelegateCommand<object> ShowProjectInfoCommand { get; private set; }
        public DelegateCommand CreateOrUpdateProjectCommand { get; private set; }
        public DelegateCommand DeleteProjectCommand { get; private set; }
        public DelegateCommand SelectImageForProjectCommand { get; private set; }
        public DelegateCommand OpenNewUsersToProjectCommand { get; private set; }
        public DelegateCommand AddUsersToProjectCommand { get; private set; }
        public DelegateCommand DeleteUsersFromProjectCommand { get; private set; }
        public DelegateCommand OpenProjectDesksPageCommand { get; private set; }

        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private Window _ownerWindow {  get; set; }
        private UsersRequestService _usersRequestService { get; set; }
        private ProjectsRequestService _projectsRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }
        private MainWindowViewModel _mainWindowViewModel { get; set; }

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

        private ClientAction _typeActionWithProject;
        public ClientAction TypeActionWithProject
        {
            get => _typeActionWithProject;
            set
            {
                _typeActionWithProject = value;
                RaisePropertyChanged(nameof(TypeActionWithProject));
            }
        }

        private List<ModelClient<ProjectModel>> _userProjects;
        public List<ModelClient<ProjectModel>> UserProjects
        {
            get => _userProjects;
            private set
            {
                _userProjects = value;
                RaisePropertyChanged(nameof(UserProjects));
            }
        }

        private ModelClient<ProjectModel> _selectedProject;
        public ModelClient<ProjectModel> SelectedProject
        {
            get =>_selectedProject;
            private set
            {
                _selectedProject = value;
                RaisePropertyChanged(nameof(SelectedProject));
                LoadProjectUsersAsync();
            }
        }

        private List<UserModel> _projectUsers = [];
        public List<UserModel> ProjectUsers
        {
            get => _projectUsers;
            set
            {
                _projectUsers = value;
                RaisePropertyChanged(nameof(ProjectUsers));
            }
        }

        public List<UserModel> NewUsersForSelectedProject {  get; set; }

        private List<UserModel> _selectedUsersForProject = [];

        public List<UserModel> SelectedUsersForProject
        {
            get => _selectedUsersForProject;
            set
            {
                _selectedUsersForProject = value;
                RaisePropertyChanged(nameof(SelectedUsersForProject));
            }
        }

        #endregion
        public ProjectsPageViewModel(AuthToken token, MainWindowViewModel mainWindowVM)
        {           
            _commonViewService = new CommonViewService();
            _projectsRequestService = new ProjectsRequestService();
            _usersRequestService = new UsersRequestService();            

            _token = token;       
            _ownerWindow = mainWindowVM.CurrentWindow;
            _mainWindowViewModel = mainWindowVM;

            OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
            OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProjectAsync);
            ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfoAsync);
            CreateOrUpdateProjectCommand = new DelegateCommand(CreateOrUpdateProjectAsync);
            DeleteProjectCommand = new DelegateCommand(DeleteProjectAsync);
            SelectImageForProjectCommand = new DelegateCommand(SelectImageForProject);
            OpenNewUsersToProjectCommand = new DelegateCommand(OpenNewUsersToProjectAsync);
            AddUsersToProjectCommand = new DelegateCommand(AddUsersToProjectAsync);
            DeleteUsersFromProjectCommand = new DelegateCommand(DeleteUsersFromProjectAsync);
            OpenProjectDesksPageCommand = new DelegateCommand(OpenProjectDesksPage);

            InitializeCurrentUserAsync();
            InitializeUserProjectsAsync();            
        }

        #region METHODS
        private async Task InitializeCurrentUserAsync()
        {
            CurrentUser = await _usersRequestService.GetCurrentUser(_token);
        }
        private async Task InitializeUserProjectsAsync()
        {
            var userProjects = (await _projectsRequestService.GetAllProjects(_token))
                .Select(project => new ModelClient<ProjectModel>(project)).ToList();

            UserProjects = userProjects;
        }
        private async Task UpdatePageAsync()
        {
            await InitializeUserProjectsAsync();
            SelectedProject = null;
            SelectedUsersForProject = [];
        }
        private async Task LoadProjectUsersAsync()
        {
            var projectUsers = new List<UserModel>();

            if (SelectedProject?.Model.UsersIds != null)
            {
                foreach (var userId in SelectedProject.Model.UsersIds)
                {
                    var user = await _usersRequestService.GetUserById(_token, userId);
                    projectUsers.Add(user);
                }
            }

            ProjectUsers = projectUsers;
        }
        private async Task LoadNewUsersForSelectedProjectAsync()
        {
            var allUsers = await _usersRequestService.GetAllUsers(_token);

            var result = allUsers.Where(user => ProjectUsers.All(u => u.Id != user.Id)).ToList();

            NewUsersForSelectedProject = result;
        }
        private void OpenNewProject()
        {
            SelectedProject = new ModelClient<ProjectModel>(new ProjectModel());

            TypeActionWithProject = ClientAction.Create;
            var window = new CreateOrUpdateProjectWindow();
            window.Owner = _ownerWindow;

            _commonViewService.OpenWindow(window, this);
        }
        private async void OpenUpdateProjectAsync(object projectId)
        {
            SelectedProject = await GetProjectClientByIdAsync(projectId);

            TypeActionWithProject = ClientAction.Update;
            var window = new CreateOrUpdateProjectWindow();
            window.Owner = _ownerWindow;

            _commonViewService.OpenWindow(window, this);
        }
        private async void ShowProjectInfoAsync(object projectId)
        {
            SelectedProject = await GetProjectClientByIdAsync(projectId);
        }
        private async Task<ModelClient<ProjectModel>> GetProjectClientByIdAsync(object projectId)
        {
            try
            {
                var selectedProject = await _projectsRequestService.GetProjectById(_token, (int)projectId);
                return new ModelClient<ProjectModel>(selectedProject);
            }
            catch(FormatException ex)
            {
                return new ModelClient<ProjectModel>(null);
            }
        }
        private async void CreateOrUpdateProjectAsync()
        {
            switch (_typeActionWithProject)
            {
                case ClientAction.Create:
                    await CreateProjectAsync();
                    break;
                case ClientAction.Update:
                    await UpdateProjectAsync();
                    break;
            }
            await UpdatePageAsync();  
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private async Task CreateProjectAsync()
        {
            var resultAction = await _projectsRequestService.CreateProject(_token, SelectedProject.Model);
            _commonViewService.ShowActionResult(resultAction, "New project created successfully");
        }
        private async Task UpdateProjectAsync()
        {
            var resultAction = await _projectsRequestService.UpdateProject(_token, SelectedProject.Model);
            _commonViewService.ShowActionResult(resultAction, "Project updated successfully");
        }
        private async void DeleteProjectAsync()
        {
            var resultAction = await _projectsRequestService.DeleteProject(_token, SelectedProject.Model.Id);
            UpdatePageAsync();
            _commonViewService.CurrentOpenWindow?.Close();
            _commonViewService.ShowActionResult(resultAction, "Project deleted successfully");
        }
        private void SelectImageForProject()
        {
            _commonViewService.SetImageForObject(SelectedProject.Model);
            SelectedProject = new ModelClient<ProjectModel>(SelectedProject.Model);
        }
        private async void OpenNewUsersToProjectAsync()
        {
            await LoadNewUsersForSelectedProjectAsync();
            var window = new AddUsersToProjectWindow();
            _commonViewService.OpenWindow(window, this);
        }
        private async void AddUsersToProjectAsync()
        {
            if (SelectedUsersForProject == null || SelectedUsersForProject?.Count == 0)
            {
                _commonViewService.ShowMessage("Select users");
                return;
            }

            var resultAction = await _projectsRequestService.AddUsersToProject(_token, SelectedProject.Model.Id, SelectedUsersForProject.Select(u => u.Id));
            _commonViewService.ShowActionResult(resultAction, "New users are added to project successfully");
            await UpdatePageAsync();
            _commonViewService.CurrentOpenWindow?.Close();
        }        
        private async void DeleteUsersFromProjectAsync()
        {
            var users = SelectedUsersForProject.Select(u => u.Email);
            var resultAction = await _projectsRequestService.RemoveUsersFromProject(_token, SelectedProject.Model.Id, SelectedUsersForProject.Select(u => u.Id));

            var deletedUsersStr = string.Join("\n", users);
            _commonViewService.ShowActionResult(resultAction, $"The following users were successfully deleted from the project:\n{deletedUsersStr}");
            await UpdatePageAsync();
        }
        private void OpenProjectDesksPage()
        {
            if (SelectedProject?.Model != null)
            {
                var page = new ProjectDesksPage();
                _mainWindowViewModel.OpenPage(page, $"Desk of {SelectedProject.Model.Name}", new ProjectDesksPageViewModel(_token, SelectedProject.Model, _mainWindowViewModel));
            }
        }
        #endregion
    }
}
