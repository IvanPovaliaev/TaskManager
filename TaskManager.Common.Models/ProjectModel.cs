using System.Collections.Generic;

namespace TaskManager.Common.Models
{
    public class ProjectModel : CommonModel
    {
        public int? AdminId { get; set; }
        public List<int> UsersIds { get; set; } = new List<int>();
        public List<int> DesksIds { get; set; } = new List<int>();
        public ProjectStatus Status { get; set; }

        public ProjectModel() { }

        public ProjectModel(string name, string description, ProjectStatus status)
        {
            Name = name;
            Description = description;
            Status = status;
        }
    }
}
