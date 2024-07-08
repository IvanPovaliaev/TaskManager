using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Common.Models;

namespace TaskManager.API.Models
{
    public class Desk : CommonObject
    {
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public string? Columns { get; set; }
        public int? AuthorId { get; set; }
        public User? Author { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public List<TaskObject> Tasks { get; set; } = new List<TaskObject>();

        public Desk() { }
        public Desk(DeskModel deskModel) : base(deskModel)
        {
            Id = deskModel.Id;
            IsPrivate = deskModel.IsPrivate;            
            AuthorId = deskModel.AuthorId;
            ProjectId = deskModel.ProjectId;
            if (deskModel.Columns.Any()) Columns = JsonConvert.SerializeObject(deskModel.Columns);
        }

        public DeskModel ToDto()
        {
            return new DeskModel()
            {
                Id = this.Id,
                AuthorId = this.AuthorId,
                Name = this.Name,
                Description = this.Description,
                CreationDate = this.CreationDate,
                Image = this.Image,
                IsPrivate = this.IsPrivate,
                Columns = JsonConvert.DeserializeObject<string[]>(this.Columns),
                ProjectId = this.ProjectId
            };
        }
    }
}
