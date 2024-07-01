using System.Linq;
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
    }
}
