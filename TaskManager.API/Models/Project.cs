using System.Collections.Generic;
using TaskManager.Common.Models;

namespace TaskManager.API.Models
{
    public class Project : CommonObject
    {
        public int Id { get; set; }
        public int? AdminId { get; set; }
        public ProjectAdmin? Admin { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Desk> Desks { get; set; } = new List<Desk>();
        public ProjectStatus Status { get; set; }
        public Project() { }
        public Project(ProjectModel projectModel) : base(projectModel)
        {
            Id = projectModel.Id;
            AdminId = projectModel.AdminId;
            Status = projectModel.Status;
        }
        public ProjectModel ToDto()
        {
            return new ProjectModel()
            {
                Id = this.Id,
                AdminId = this.AdminId,
                Name = this.Name,
                Description = this.Description,
                CreationDate = this.CreationDate,
                Image = this.Image,
                Status = this.Status,                
            };
        }

        public override CommonModel ToShortDto()
        {
            return new CommonModel()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                CreationDate = this.CreationDate,
                Image = this.Image
            };
        }
    }
}
