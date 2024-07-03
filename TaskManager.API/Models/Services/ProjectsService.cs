using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.API.Models.Abstractions;
using TaskManager.API.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.API.Models.Services
{
    public class ProjectsService : AbstractService, ICommonService<ProjectModel>
    {
        private readonly ApplicationContext _db;

        public ProjectsService(ApplicationContext db)
        {
            _db = db;
        }
        public bool Create(ProjectModel model)
        {
            return DoAction(() =>
            {
                var newProject = new Project(model);
                _db.Projects.Add(newProject);
                _db.SaveChanges();
            });            
        }

        public bool Remove(int id)
        {
            return DoAction(() =>
            {
                var remProject = _db.Projects.FirstOrDefault(project => project.Id == id);
                _db.Projects.Remove(remProject);
                _db.SaveChanges();
            });
        }

        public bool Update(int id, ProjectModel model)
        {
            return DoAction(() =>
            {
                var currentProject = _db.Projects.FirstOrDefault(project => project.Id == id);
                currentProject.Name = model.Name;
                currentProject.Description = model.Description;
                currentProject.Image = model.Image;
                currentProject.Status = model.Status;
                currentProject.AdminId = model.AdminId;
                _db.Projects.Update(currentProject);
                _db.SaveChanges();
            });
        }

        public ProjectModel Get(int id)
        {
            return _db.Projects.FirstOrDefault(p => p.Id == id)?.ToDto();
        }
        public async Task<IEnumerable<ProjectModel>> GetByUserId(int userId)
        {
            var result = new List<ProjectModel>();
            var admin = _db.ProjectAdmins.FirstOrDefault(a => a.UserId == userId);
            if (admin != null)
            {
                var projects = await _db.Projects.Where(p => p.AdminId == admin.Id).Select(p => p.ToDto()).ToListAsync();
                result.AddRange(projects);
            }
            var projectsForUser = await _db.Projects.Include(p => p.Users).Where(p => p.Id == userId).Select(p => p.ToDto()).ToListAsync();
            result.AddRange(projectsForUser);
            return result;
        }

        public IQueryable<ProjectModel> GetAll()
        {
            return _db.Projects.Select(p => p.ToDto());
        }

        public void AddUsersToProject(int id, IEnumerable<int> userIds)
        {
            var project = _db.Projects.FirstOrDefault(p => p.Id == id);
            foreach (var userId in userIds)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == userId);
                project.Users.Add(user);
            }
            _db.SaveChanges();
        }

        public void RemoveUsersFromProject(int id, IEnumerable<int> userIds)
        {
            var project = _db.Projects.Include(p => p.Users).FirstOrDefault(p => p.Id == id);
            foreach (var userId in userIds)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == userId);
                if (project.Users.Contains(user)) project.Users.Remove(user);
            }
            _db.SaveChanges();
        }
    }
}
