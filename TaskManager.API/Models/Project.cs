using System.Collections.Generic;

namespace TaskManager.API.Models
{
    public class Project : CommonObject
    {
        public List<User>? Users { get; set; }
        public List<Desk>? Desks { get; set; }
    }
}
