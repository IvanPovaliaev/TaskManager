using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class LoginViewModel : BindableBase
    {

        private UsersRequestService _usersRequestService;

        #region COMMAND
        public DelegateCommand<object> GetUserFromDBCommand { get; private set; }
        #endregion

        public LoginViewModel()
        {
            _usersRequestService = new UsersRequestService();

            GetUserFromDBCommand = new DelegateCommand<object>(GetUserFromDB);
        }

        #region PROPERIES
        public string UserLogin { get; set; }
        public string UserPassword { get; private set; }

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
            UserPassword = passBox.Password;
            AuthToken = await _usersRequestService.GetToken(UserLogin, UserPassword);

            if (AuthToken == null) return;

            CurrentUser = await _usersRequestService.GetCurrentUser(AuthToken);
            if (CurrentUser != null)
                MessageBox.Show(CurrentUser.FirstName);
        }
        #endregion
    }
}
