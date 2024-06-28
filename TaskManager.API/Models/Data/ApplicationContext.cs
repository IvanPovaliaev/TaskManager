using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace TaskManager.API.Models.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ProjectAdmin> ProjectAdmins { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
            if (!Users.Any(user => user.Role == UserRole.Admin))
            {
                Users.Add(new User("Admin", "Admin", "admin@admin.com", "qwerty123", UserRole.Admin));
                SaveChanges();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("DefaultConnection")
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        }
    }
}
