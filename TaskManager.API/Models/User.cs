using System;
using System.Collections.Generic;
using TaskManager.Common.Models;

namespace TaskManager.API.Models
{
    public class User
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
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Desk> Desks { get; set; } = new List<Desk>();
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
        public UserRole Role { get; set; }

        public User() { }
        public User(string fName, string sName, string email, string password,
            UserRole role = UserRole.User, string? phone = null, byte[]? photo = null)
        {
            FirstName = fName;
            Surname = sName;
            Email = email;
            Password = password;
            Phone = phone;
            Photo = photo;
            RegistrationDate = DateTime.Now;
            Role = role;
        }
        public User(UserModel model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            Surname = model.Surname;
            Email = model.Email;
            Password = model.Password;
            Phone = model.Phone;
            Photo = model.Photo;
            RegistrationDate = model.RegistrationDate;
            Role = model.Role;
        }

        public UserModel ToDto()
        {
            return new UserModel()
            {
                Id = this.Id,
                FirstName = this.FirstName,
                Surname = this.Surname,
                Email = this.Email,
                Password = this.Password,
                Phone = this.Phone,
                Photo = this.Photo,
                RegistrationDate = this.RegistrationDate,
                Role = this.Role
            };
        }
    }
}
