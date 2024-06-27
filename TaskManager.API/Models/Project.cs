using System.Collections.Generic;

namespace TaskManager.API.Models
{
    public class Project : CommonObject
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public ProjectAdmin Admin { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Desk> Desks { get; set; } = new List<Desk>();
        public ProjectStatus Status { get; set; }
    }
}
