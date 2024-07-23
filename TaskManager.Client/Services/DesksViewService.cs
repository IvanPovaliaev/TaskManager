﻿using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class DesksViewService
    {
        private AuthToken _token { get; set; }
        private CommonViewService _commonViewService { get; set; }
        private DesksRequestService _desksRequestService { get; set; }
        

        public DesksViewService(AuthToken token, DesksRequestService desksRequestService, CommonViewService commonViewService)
        {
            _token = token;
            _desksRequestService = desksRequestService;
            _commonViewService = commonViewService;
        }

        public async Task<ModelClient<DeskModel>> GetDeskClientByIdAsync(object deskId)
        {
            try
            {
                var selectedDesk = await _desksRequestService.GetDeskById(_token, (int)deskId);
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
            var resultAction = await _desksRequestService.UpdateDesk(_token, desk);
            _commonViewService.ShowActionResult(resultAction, "Desk updated successfully");
        }
        public async Task DeleteDeskAsync(int deskId)
        {
            var resultAction = await _desksRequestService.DeleteDesk(_token, deskId);
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