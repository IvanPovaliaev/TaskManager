using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TaskManager.Client.Models;
using TaskManager.Client.Models.Extensions;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {        
        #region COMMANDS
        public DelegateCommand OpenMyInfoPageCommand { get; set; }
        public DelegateCommand OpenProjectPageCommand { get; set; }
        public DelegateCommand OpenDesksPageCommand { get; set; }
        public DelegateCommand OpenTasksPageCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand OpenUsersManagementCommand { get; set; }
        #endregion

        #region PROPERTIES
        private CommonViewService _commonViewService { get; set; }

        private readonly string _userInfoButtonName = "My info";
        private readonly string _userProjectsButtonName = "My projects";
        private readonly string _userDesksButtonName = "My desks";
        private readonly string _userTasksButtonName = "My tasks";
        private readonly string _logoutButtonName = "Logout";

        private readonly string _manageUsersButtonName = "Users";

        private Window _currentWindow;
        public Window CurrentWindow
        {
            get => _currentWindow;
            private set
            {
                _currentWindow = value;
                RaisePropertyChanged(nameof(CurrentWindow));
            }
        }

        private AuthToken _token;
        public AuthToken Token
        {
            get => _token;
            private set
            {
                _token = value;
                RaisePropertyChanged(nameof(Token));
            }
        }

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

        private Dictionary<string, DelegateCommand> _navButtons = new Dictionary<string, DelegateCommand>();
        public Dictionary<string, DelegateCommand> NavButtons
        {
            get => _navButtons;
            private set
            {
                _navButtons = value;
                RaisePropertyChanged(nameof(NavButtons));
            }
        }

        private string _selectedPageName;
        public string SelectedPageName
        {
            get => _selectedPageName;
            set 
            {
                _selectedPageName = value;
                RaisePropertyChanged(nameof(SelectedPageName));
            }
        }

        private Page _selectedPage;
        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                _selectedPage = value;
                RaisePropertyChanged(nameof(SelectedPage));
            }
        }
        private BitmapImage _currentUserPhoto;
        public BitmapImage CurrentUserPhoto
        {
            get => _currentUserPhoto;
            set
            {
                _currentUserPhoto = value;
                RaisePropertyChanged(nameof(CurrentUserPhoto));
            }
        }

        #endregion

        public MainWindowViewModel(AuthToken token, UserModel currentUser, Window currentWindow = null)
        {
            _commonViewService = new CommonViewService();

            Token = token;
            CurrentUser = currentUser;
            CurrentUserPhoto = CurrentUser.LoadPhoto();
            _currentWindow = currentWindow;

            OpenMyInfoPageCommand = new DelegateCommand(OpenMyInfoPage);
            NavButtons.Add(_userInfoButtonName, OpenMyInfoPageCommand);

            OpenProjectPageCommand = new DelegateCommand(OpenProjectsPage);
            NavButtons.Add(_userProjectsButtonName, OpenProjectPageCommand);

            OpenDesksPageCommand = new DelegateCommand(OpenDesksPage);
            NavButtons.Add(_userDesksButtonName, OpenDesksPageCommand);

            OpenTasksPageCommand = new DelegateCommand(OpenTasksPage);
            NavButtons.Add(_userTasksButtonName, OpenTasksPageCommand);

            if (currentUser.Role == UserRole.Admin)
            {
                OpenUsersManagementCommand = new DelegateCommand(OpenUsersManagement);
                NavButtons.Add(_manageUsersButtonName, OpenUsersManagementCommand);
            }

            LogoutCommand = new DelegateCommand(Logout);
            NavButtons.Add(_logoutButtonName, LogoutCommand);

            OpenMyInfoPage();
        }

        #region METHODS
        private void OpenMyInfoPage()
        {
            var page = new UserInfoPage();
            OpenPage(page, _userInfoButtonName, this);
        }
        private void OpenProjectsPage()
        {
            var page = new ProjectsPage();
            var model = new ProjectsPageViewModel(_token, this);
            OpenPage(page, _userProjectsButtonName, model);
        }

        private void OpenDesksPage()
        {
            var page = new UserDesksPage();
            var model = new UserDesksPageViewModel(_token, this);
            OpenPage(page, _userDesksButtonName, model);
        }

        private void OpenTasksPage()
        {
            var page = new UserTasksPage();
            var model = new UserTasksPageViewModel(_token);
            OpenPage(page, _userTasksButtonName, model);
        }

        private void Logout()
        {
            var question = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo);
            if (question == MessageBoxResult.Yes)
            {
                var login = new Login();
                login.Show();
                _currentWindow?.Close();
            }
        }

        private void OpenUsersManagement()
        {
            var page = new UsersPage();
            var model = new UsersPageViewModel(_token, page, this);
            OpenPage(page, _manageUsersButtonName, model);
        }

        #endregion

        public void OpenPage(Page page, string pageName, BindableBase viewModel)
        {
            SelectedPage = page;
            SelectedPageName = pageName;
            SelectedPage.DataContext = viewModel;
        }
    }
}
