using System.Collections.Generic;

namespace TaskManager.Common.Models
{
    public class ProjectModel : CommonModel
    {
        public int? AdminId { get; set; }
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<DeskModel> Desks { get; set; } = new List<DeskModel>();
        public ProjectStatus Status { get; set; }
    }
}
