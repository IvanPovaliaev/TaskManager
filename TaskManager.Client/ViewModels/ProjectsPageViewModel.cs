using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectsPageViewModel : BindableBase
    {
        #region COMMANDS

        public DelegateCommand OpenNewProjectCommand;
        public DelegateCommand<object> OpenUpdateProjectCommand;
        public DelegateCommand<object> ShowProjectInfoCommand;

        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private UsersRequestService _usersRequestService { get; set; }
        private ProjectsRequestService _projectsRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }

        private List<ModelClient<ProjectModel>> _userProjects = new List<ModelClient<ProjectModel>>();
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

                var projectUsers = new List<UserModel>();

                SelectedProject.Model.UsersIds?.ForEach(async userId =>
                    {
                        var s = await _usersRequestService.GetUserById(_token, userId);
                        projectUsers.Add(s);
                    });

                ProjectUsers = projectUsers;
            }
        }

        private List<UserModel> _projectUsers = [];
        public List<UserModel> ProjectUsers
        {
            get => _projectUsers;
            set
            {
                _projectUsers = value;
                RaisePropertyChanged(nameof(_projectUsers));
            }
        }
        #endregion

        public ProjectsPageViewModel(AuthToken token)
        {
            _commonViewService = new CommonViewService();
            _projectsRequestService = new ProjectsRequestService();
            _usersRequestService = new UsersRequestService();

            _token = token;            

            OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
            OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProject);
            ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfo);
            InitializeUserProjectsAsync();            
        }

        #region METHODS
        private void OpenNewProject()
        {
            _commonViewService.ShowMessage(nameof(OpenNewProject));
        }
        private void OpenUpdateProject(object param)
        {
            SelectedProject = (ModelClient<ProjectModel>)param;
            _commonViewService.ShowMessage(nameof(OpenUpdateProject));
        }

        private void ShowProjectInfo(object param)
        {
            SelectedProject = (ModelClient<ProjectModel>)param;
            _commonViewService.ShowMessage(nameof(ShowProjectInfo));
        }

        private async void InitializeUserProjectsAsync()
        {
            var userProjects = (await _projectsRequestService.GetAllProjects(_token))
                .Select(project => new ModelClient<ProjectModel>(project));
            UserProjects = userProjects.ToList();
        }
        #endregion
    }
}
