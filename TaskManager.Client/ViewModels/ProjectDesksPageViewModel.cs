using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectDesksPageViewModel : BindableBase
    {
        #region COMMAND

        #endregion

        #region PROPERTIES
        private AuthToken _token { get; set; }
        private ProjectModel _project { get; set; }
        private DesksRequestService _desksRequestService { get; set; }

        private List<ModelClient<DeskModel>> _projectDesks = [];

        public List<ModelClient<DeskModel>> ProjectDesks
        {
            get => _projectDesks;
            set
            {
                _projectDesks = value;
                RaisePropertyChanged(nameof(ProjectDesks));
            }
        }

        #endregion

        public ProjectDesksPageViewModel(AuthToken token, ProjectModel project)
        {
            _token = token;
            _project = project;
            _desksRequestService = new DesksRequestService();

            LoadProjectDesksAsync();
        }

        #region METHODS
        private async Task LoadProjectDesksAsync()
        {
            var desks = await _desksRequestService.GetDesksByProject(_token, _project.Id);

            ProjectDesks = desks?.Select(desk => new ModelClient<DeskModel>(desk)).ToList();
        }
        #endregion
    }
}
