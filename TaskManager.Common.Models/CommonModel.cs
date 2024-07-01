using System;

namespace TaskManager.Common.Models
{
    public class CommonModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public byte[]? Image { get; set; }
    }
}
