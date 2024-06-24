using System;

namespace TaskManager.API.Models
{
    public class Desk : CommonObject
    {
        public bool IsPrivate { get; set; }
        public string? Columns { get; set; }
        public User? Author { get; set; }

    }
}
