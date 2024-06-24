using System;

namespace TaskManager.API.Models
{
    public class User
    {
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDay { get; set; }
        public byte[]? Photo { get; set; }
    }
}
