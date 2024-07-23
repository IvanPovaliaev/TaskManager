using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using Newtonsoft.Json.Linq;
using TaskManager.Common.Models;
using Prism.Commands;
using TaskManager.Client.Views.AddWindows;
using System.Windows;
using System.Collections.ObjectModel;

namespace TaskManager.Client.ViewModels
{
    public class UserDesksPageViewModel : BindableBase
    {
        #region COMMANDS
        public DelegateCommand OpenUpdateDeskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
        public DelegateCommand DeleteDeskCommand { get; private set; }
        public DelegateCommand SelectImageForDeskCommand { get; private set; }
        public DelegateCommand AddNewColumnItemCommand { get; private set; }
        public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }
        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private Window _ownerWindow { get; set; }
        private DesksRequestService _desksRequestService { get; set; }
        private UsersRequestService _usersRequestService { get; set; }
        private CommonViewService _commonViewService { get; set; }
        private DesksViewService _deskViewService { get; set; }

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

        private List<ModelClient<DeskModel>> _alDesks = new();
        public List<ModelClient<DeskModel>> AllDesks
        {
            get => _alDesks;
            set
            {
                _alDesks = value;
                RaisePropertyChanged(nameof(AllDesks));
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

        private Dictionary<string, DelegateCommand> _contextMenuCommands;
        public Dictionary<string, DelegateCommand> ContextMenuCommands
        {
            get =>_contextMenuCommands;
            set
            {
                _contextMenuCommands = value;
                RaisePropertyChanged(nameof(ContextMenuCommands));
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

        public UserDesksPageViewModel(AuthToken token, MainWindowViewModel mainWindowVM)
        {
            _token = token;
            _ownerWindow = mainWindowVM.CurrentWindow;
            _desksRequestService = new DesksRequestService();
            _usersRequestService = new UsersRequestService();
            _commonViewService = new CommonViewService();
            _deskViewService = new DesksViewService(token, _desksRequestService, _commonViewService);

            InitializeAllDesksAsync();

            OpenUpdateDeskCommand = new DelegateCommand(OpenUpdateDeskAsync);
            DeleteDeskCommand = new DelegateCommand(DeleteDeskAsync);
            CreateOrUpdateDeskCommand = new DelegateCommand(UpdateDeskAsync);
            SelectImageForDeskCommand = new DelegateCommand(() =>
            {
                var selectedDesk = SelectedDesk;
                _deskViewService.SelectImageForDesk(ref selectedDesk);
                SelectedDesk = selectedDesk;
            });
            AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
            RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);

            ContextMenuCommands = new Dictionary<string, DelegateCommand>();
            ContextMenuCommands["Edit"] = OpenUpdateDeskCommand;
            ContextMenuCommands["Delete"] = DeleteDeskCommand;            
        }

        #region METHODS
        private async Task InitializeAllDesksAsync()
        {
            var desks = await _desksRequestService.GetDesksForCurrentUser(_token);

            AllDesks = desks?.Select(desk => new ModelClient<DeskModel>(desk)).ToList();
        }
        private async Task UpdatePage()
        {
            await InitializeAllDesksAsync();
            SelectedDesk = null;
            _commonViewService.CurrentOpenWindow?.Close();
        }
        private async void OpenUpdateDeskAsync()
        {
            TypeActionWithDesk = ClientAction.Update;

            SelectedDesk = await _deskViewService.GetDeskClientByIdAsync(SelectedDesk.Model.Id);

            ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelper>(SelectedDesk.Model.Columns.Select(c => new ColumnBindingHelper(c)));

            _deskViewService.OpenViewDeskInfo(SelectedDesk.Model.Id, this, _ownerWindow);
        }
        private async void DeleteDeskAsync()
        {
            await _deskViewService.DeleteDeskAsync(SelectedDesk.Model.Id);
            await UpdatePage();
        }
        private async void UpdateDeskAsync()
        {
            SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
            await _deskViewService.UpdateDeskAsync(SelectedDesk.Model);
            await UpdatePage();
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
