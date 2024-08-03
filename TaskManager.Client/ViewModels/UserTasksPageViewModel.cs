using Prism.Mvvm;
using System.Collections.Generic;
using TaskManager.Client.Models;
using TaskManager.Client.Services;

namespace TaskManager.Client.ViewModels
{
    public class UserTasksPageViewModel : BindableBase
    {
        private AuthToken _token {  get; set; }
        private TasksRequestService _tasksRequestService { get; set; }
        private UsersRequestService _usersRequestService { get; set; }

        private List<TaskClient> _allTasks;
        public List<TaskClient> AllTasks
        {
            get => _allTasks;
            private set
            {
                _allTasks = value;
                RaisePropertyChanged(nameof(AllTasks));
            }
        }

        public UserTasksPageViewModel(AuthToken token)
        {
            _token = token;
            _tasksRequestService = new TasksRequestService();
            _usersRequestService = new UsersRequestService();

            InitializeAllTasksAsync();
        }             

        private async void InitializeAllTasksAsync()
        {
            var tasks = await _tasksRequestService.GetTasksForCurrentUserAsync(_token);

            var allTasks = new List<TaskClient>();

            foreach (var task in tasks)
            {
                var taskClient = new TaskClient(task);
                if (task.CreatorId != null)
                    taskClient.Creator = await _usersRequestService.GetUserByIdAsync(_token, (int)(task.CreatorId));

                if (task.ExecutorId != null)
                    taskClient.Executor = await _usersRequestService.GetUserByIdAsync(_token, (int)(task.ExecutorId));

                allTasks.Add(taskClient);
            }
            AllTasks = allTasks;
        }
    }
}
