using System;

namespace TaskManager.API.Models
{
    public class TaskModel : CommonObject
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte[]? File { get; set; }

    }
}
