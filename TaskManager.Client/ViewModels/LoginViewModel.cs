using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private UsersRequestService _usersRequestService { get; set; }  
        private CommonViewService _commonViewService { get; set; }

        #region COMMANDS
        public DelegateCommand<object> GetUserFromDBCommand { get; private set; }
        public DelegateCommand<object> LoginFromCacheCommand { get; private set; }
        #endregion

        #region PROPERIES
        private string _cachePath = Path.GetTempPath() + "userTaskManager.txt";

        private Window _currentWindow;

        public string UserLogin { get; set; }
        public string UserPassword { get; private set; }

        private UserCache _currentUserCache;
        public UserCache CurrentUserCache
        {
            get => _currentUserCache;
            set
            {
                _currentUserCache = value;
                RaisePropertyChanged(nameof(CurrentUserCache));
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

        private AuthToken _authToken;
        public AuthToken AuthToken
        {
            get => _authToken;
            set
            {
                _authToken = value;
                RaisePropertyChanged(nameof(AuthToken));
            }
        }

        private Login _loginWindow;
        public Login LoginWindow
        {
            get => _loginWindow;
            set
            {
                _loginWindow = value;
                LoginWindow.UserPassword.PasswordChanged += PasswordChanged;
                RaisePropertyChanged(nameof(LoginWindow));
            }
        }
        #endregion

        public LoginViewModel()
        {
            _usersRequestService = new UsersRequestService();
            _commonViewService = new CommonViewService();

            CurrentUserCache = GetUserCache();

            GetUserFromDBCommand = new DelegateCommand<object>(GetUserFromDB);
            LoginFromCacheCommand = new DelegateCommand<object>(LoginFromCache);            
        }

        #region EVENTS METHODS

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pasBox = (PasswordBox)sender;
            if (pasBox.Password.Length > 0) LoginWindow.PasswordPlaceholder.Visibility = Visibility.Collapsed;
            else LoginWindow.PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        #endregion

        #region METHODS
        private async void GetUserFromDB(object parameter)
        {
            var passBox = (PasswordBox)parameter;

            var isNewUser = false;

            _currentWindow = Window.GetWindow(passBox);

            if (UserLogin != CurrentUserCache?.Login || UserPassword != CurrentUserCache?.Password)
                isNewUser = true;

            UserPassword = passBox.Password;
            AuthToken = await _usersRequestService.GetTokenAsync(UserLogin, UserPassword);

            if (AuthToken == null)
            {
                _commonViewService.ShowMessage("User not found!");
                return;
            }

            CurrentUser = await _usersRequestService.GetCurrentUserAsync(AuthToken);

            if (isNewUser && CurrentUser != null)
            {
                var saveUserCacheMsg = MessageBox.Show("Do you want to save your login and password?", "Data save", MessageBoxButton.YesNo);

                if (saveUserCacheMsg == MessageBoxResult.Yes)
                {
                    var newUserCache = new UserCache();
                    newUserCache.Login = UserLogin;
                    newUserCache.Password = UserPassword;
                    CreateUserCache(newUserCache);
                }
                OpenMainWindow();
            }            
        }

        private void  CreateUserCache(UserCache userCache)
        {
            string jsonUserCache = JsonConvert.SerializeObject(userCache);
            using (var sw = new StreamWriter(_cachePath, false, System.Text.Encoding.Default))
            {
                sw.Write(jsonUserCache);
                MessageBox.Show("Success");
            }
        }

        private UserCache GetUserCache()
        {
            var isCacheExist = File.Exists(_cachePath);

            if (isCacheExist && File.ReadAllText(_cachePath).Length != 0)
                return JsonConvert.DeserializeObject<UserCache>(File.ReadAllText(_cachePath));

            return null;
        }

        private async void LoginFromCache(object window)
        {
            _currentWindow = (Window)window;
            UserLogin = CurrentUserCache.Login;
            UserPassword = CurrentUserCache.Password;
            AuthToken = await _usersRequestService.GetTokenAsync(UserLogin, UserPassword);

            CurrentUser = await _usersRequestService.GetCurrentUserAsync(AuthToken);
            if (CurrentUser != null)
                OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(AuthToken, CurrentUser, mainWindow);
            mainWindow.Show();
            _currentWindow.Close();
        }
        #endregion
    }
}
