using TaskManager.Common.Models;

namespace TaskManager.Client.Models
{
    public class TaskClient : ModelClient<TaskModel>
    {
        public UserModel Creator { get; set; }
        public UserModel Executor { get; set; }
        public bool IsHaveFile { get => Model?.File != null; }

        public TaskClient(TaskModel model) : base(model)
        {
        }
    }
}
