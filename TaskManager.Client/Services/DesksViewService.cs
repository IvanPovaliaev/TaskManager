using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;
using TaskManager.Common.Models.Services;

namespace TaskManager.Client.Services
{
    public class DesksViewService
    {
        private AuthToken _token { get; set; }
        private CommonViewService _commonViewService { get; set; }
        private DesksRequestService _desksRequestService { get; set; }    
        private ValidationService _validationService { get; set; }

        public DesksViewService(AuthToken token, DesksRequestService desksRequestService, CommonViewService commonViewService)
        {
            _token = token;
            _desksRequestService = desksRequestService;
            _commonViewService = commonViewService;
            _validationService = new ValidationService();
        }

        public async Task<ModelClient<DeskModel>> GetDeskClientByIdAsync(object deskId)
        {
            try
            {
                var selectedDesk = await _desksRequestService.GetDeskByIdAsync(_token, (int)deskId);
                return new ModelClient<DeskModel>(selectedDesk);
            }
            catch (FormatException ex)
            {
                return new ModelClient<DeskModel>(null);
            }
        }
        public void OpenViewDeskInfo(object deskId, BindableBase context, Window ownerWindow = null)
        {
            var window = new CreateOrUpdateDeskWindow();
            window.Owner = ownerWindow;

            _commonViewService.OpenWindow(window, context);
        }
        public async Task UpdateDeskAsync(DeskModel desk)
        {
            var isCorrectInput = _validationService.IsCorrectDeskInputData(desk, out var messages);

            if (!isCorrectInput)
            {
                _commonViewService.ShowMessage(string.Join("\n", messages));
                return;
            }

            var resultAction = await _desksRequestService.UpdateDeskAsync(_token, desk);
            _commonViewService.ShowActionResult(resultAction.StatusCode, "Desk updated successfully");
            _commonViewService.CurrentOpenWindow?.Close();
        }
        public async Task DeleteDeskAsync(int deskId)
        {
            var resultAction = await _desksRequestService.DeleteDeskAsync(_token, deskId);
            _commonViewService.CurrentOpenWindow?.Close();
            _commonViewService.ShowActionResult(resultAction, "Desk deleted successfully");
        }
        public void SelectImageForDesk(ref ModelClient<DeskModel> selectedDesk)
        {
            if (selectedDesk?.Model == null) return;
            _commonViewService.SetImageForObject(selectedDesk.Model);
            selectedDesk = new ModelClient<DeskModel>(selectedDesk.Model);
        }
    }
}
