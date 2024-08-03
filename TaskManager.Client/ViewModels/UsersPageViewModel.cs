using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class UsersPageViewModel : BindableBase
    {
        #region COMMANDS
        public DelegateCommand<object> OpenUpdateUserCommand { get; private set; }
        public DelegateCommand OpenNewUserCommand { get; private set; }
        public DelegateCommand<object> DeleteUserCommand { get; private set; }
        public DelegateCommand CreateOrUpdateUserCommand { get; private set; }
        public DelegateCommand OpenSelectUsersFromExcelCommand { get; private set; }
        public DelegateCommand GetUsersFromExcelCommand { get; private set; }
        public DelegateCommand AddUsersFromExcelCommand { get; private set; }
        public DelegateCommand SelectPhotoForUserCommand { get; private set; }
        #endregion

        #region PROPERTIES
        private AuthToken _authToken {  get; set; }
        private Window _ownerWindow { get; set; }
        private const string _excelDialogFilterPattern = "Excel Files(.xls)|*.xls| Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm";
        private UsersRequestService _usersRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }
        private ExcelService _excelService { get; set; }

        private List<UserModelClient> _allUsers;
        public List<UserModelClient> AllUsers
        {
            get => _allUsers;
            set
            {
                _allUsers = value;
                RaisePropertyChanged(nameof(AllUsers));
            }
        }

        private List<UserModel> _UsersFromExcel;
        public List<UserModel> UsersFromExcel
        {
            get => _UsersFromExcel;
            set
            {
                _UsersFromExcel = value;
                RaisePropertyChanged(nameof(UsersFromExcel));
            }
        }

        private List<UserModel> _selectedUsersFromExcel = [];
        public List<UserModel> SelectedUsersFromExcel
        {
            get => _selectedUsersFromExcel;
            set
            {
                _selectedUsersFromExcel = value;
                RaisePropertyChanged(nameof(SelectedUsersFromExcel));
            }
        }

        private UserModelClient _selectedUser;
        public UserModelClient SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser));
            }
        }

        private ClientAction _typeActionWithUser;
        public ClientAction TypeActionWithUser
        {
            get => _typeActionWithUser;
            set
            {
                _typeActionWithUser = value;
                RaisePropertyChanged(nameof(TypeActionWithUser));
            }
        }

        #endregion

        public UsersPageViewModel(AuthToken token, UsersPage usersPage, MainWindowViewModel? mainWindowVM = null)
        {
            _authToken = token;
            _ownerWindow = mainWindowVM?.CurrentWindow;

            _usersRequestService = new UsersRequestService();
            _commonViewService = new CommonViewService();
            _excelService = new ExcelService();

            OpenUpdateUserCommand =  new DelegateCommand<object>(OpenUpdateUserAsync);
            OpenNewUserCommand =  new DelegateCommand(OpenNewUser);
            DeleteUserCommand =  new DelegateCommand<object>(DeleteUserAsync);
            CreateOrUpdateUserCommand =  new DelegateCommand(CreateOrUpdateUser);
            OpenSelectUsersFromExcelCommand =  new DelegateCommand(OpenSelectUsersFromExcel);
            GetUsersFromExcelCommand =  new DelegateCommand(GetUsersFromExcel);
            AddUsersFromExcelCommand =  new DelegateCommand(AddUsersFromExcelAsync);
            SelectPhotoForUserCommand = new DelegateCommand(SelectPhotoForUser);

            usersPage.Loaded += LoadAllUsersAsync;
        }

        #region EVENTS
        private async void LoadAllUsersAsync(object sender, RoutedEventArgs e)
        {
            var allUsers = await _usersRequestService.GetAllUsers(_authToken);
            AllUsers = allUsers.Select(user => new UserModelClient(user)).ToList();
        }
        #endregion

        #region METHODS
        private void UpdatePage()
        {
            SelectedUser = null;
            SelectedUsersFromExcel = [];
            AllUsers = [];
            LoadAllUsersAsync(this, new RoutedEventArgs());
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private async void OpenUpdateUserAsync(object userId)
        {
            if (userId == null) return;
            TypeActionWithUser = ClientAction.Update;

            var selectedUser = await _usersRequestService.GetUserById(_authToken, (int)userId);

            SelectedUser = new UserModelClient(selectedUser);

            var wnd = new CreateOrUpdateUserWindow();
            wnd.Owner = _ownerWindow;
            _commonViewService.OpenWindow(wnd, this);
        }
        private void OpenNewUser()
        {
            TypeActionWithUser = ClientAction.Create;
            SelectedUser = new UserModelClient(new UserModel());

            var wnd = new CreateOrUpdateUserWindow();
            wnd.Owner = _ownerWindow;
            _commonViewService.OpenWindow(wnd, this);
        }
        private async void DeleteUserAsync(object userId)
        {
            if (userId !=  null)
            {
                await _usersRequestService.DeleteUser(_authToken, (int)userId);
                UpdatePage();
            }
                
        }
        private async void CreateOrUpdateUser()
        {
            switch (TypeActionWithUser)
            {
                case ClientAction.Create:
                    await _usersRequestService.CreateUser(_authToken, SelectedUser.Model);
                    break;
                case ClientAction.Update:
                    await _usersRequestService.UpdateUser(_authToken, SelectedUser.Model);
                    break;
            }
            UpdatePage();            
        }
        private void OpenSelectUsersFromExcel()
        {
            var wnd = new UsersFromExcelWindow();
            wnd.Owner = _ownerWindow;
            _commonViewService.OpenWindow(wnd, this);
        }
        private void GetUsersFromExcel()
        {
            var filePath = _commonViewService.GetFileFromDialog(_excelDialogFilterPattern);

            if (string.IsNullOrEmpty(filePath)) return;

            UsersFromExcel = _excelService.GetAllUsers(filePath);
        }
        private async void AddUsersFromExcelAsync()
        {
            if (SelectedUsersFromExcel != null && SelectedUsersFromExcel.Count != 0)
            {
                var result = await _usersRequestService.CreateMultipleUsers(_authToken, SelectedUsersFromExcel);
                _commonViewService.ShowActionResult(result, "All users have been created successfully");
            }
            UpdatePage();
        }

        public void SelectPhotoForUser()
        {
            if (SelectedUser?.Model == null) return;

            var selectedUser = SelectedUser.Model;
            _commonViewService.SetPhotoForUser(selectedUser);

            SelectedUser = new UserModelClient(selectedUser);
        }

        #endregion
    }
}
