using System;

namespace TaskManager.Common.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDay { get; set; }
        public byte[]? Photo { get; set; }
        public UserRole Role { get; set; }

        public UserModel() { }
        public UserModel(string fName, string sName, string email, string password,
            UserRole role, string phone)
        {
            FirstName = fName;
            Surname = sName;
            Email = email;
            Password = password;
            Phone = phone;
            RegistrationDate = DateTime.Now;
            Role = role;
        }
    }
}
