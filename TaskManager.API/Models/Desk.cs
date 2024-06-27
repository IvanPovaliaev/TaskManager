using System;

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
        public Project? Project { get; set; }
    }
}
