﻿using Newtonsoft.Json;
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

        private UsersRequestService _usersRequestService;        

        #region COMMAND
        public DelegateCommand<object> GetUserFromDBCommand { get; private set; }
        public DelegateCommand<object> LoginFromCacheCommand { get; private set; }
        #endregion

        public LoginViewModel()
        {
            _usersRequestService = new UsersRequestService();
            CurrentUserCache = GetUserCache();

            GetUserFromDBCommand = new DelegateCommand<object>(GetUserFromDB);
            LoginFromCacheCommand = new DelegateCommand<object>(LoginFromCache);
        }

        #region PROPERIES
        private string _cachePath = Path.GetTempPath() +"userTaskManager.txt";

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
            get =>_currentUser;
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
            AuthToken = await _usersRequestService.GetToken(UserLogin, UserPassword);

            if (AuthToken == null) return;

            CurrentUser = await _usersRequestService.GetCurrentUser(AuthToken);

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
                MessageBox.Show("Succes");
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
            AuthToken = await _usersRequestService.GetToken(UserLogin, UserPassword);

            CurrentUser = await _usersRequestService.GetCurrentUser(AuthToken);
            if (CurrentUser != null)
                OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _currentWindow.Close();
        }
        #endregion
    }
}
