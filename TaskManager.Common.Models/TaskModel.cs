using System;

namespace TaskManager.Common.Models
{
    public class TaskModel : CommonModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte[]? File { get; set; }
        public int DeskId { get; set; }
        public string Column { get; set; }
        public int? CreatorId { get; set; }
        public int? ExecutorId { get; set; }

        public TaskModel() { }

        public TaskModel(string name, string description, DateTime start, DateTime end, string column)
        {
            Name = name;
            Description = description;
            StartDate = start;
            EndDate = end;
            Column = column;
        }
    }
}