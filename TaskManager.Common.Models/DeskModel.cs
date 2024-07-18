using System.Collections.Generic;
using System.Linq;

namespace TaskManager.Common.Models
{
    public class DeskModel : CommonModel
    {
        public bool IsPrivate { get; set; }
        public string[]? Columns { get; set; }
        public int ProjectId { get; set; }
        public int? AuthorId { get; set; }
        public List<int>? TasksIds { get; set; }

        public DeskModel() { }

        public DeskModel(string name, string description, bool isPrivate, IEnumerable<string> columns)
        {
            Name = name;
            Description = description;
            IsPrivate = isPrivate;
            Columns = columns.ToArray();
        }
    }
}
