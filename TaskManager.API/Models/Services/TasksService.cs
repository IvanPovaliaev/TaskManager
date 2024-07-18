using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Linq;
using TaskManager.API.Models.Abstractions;
using TaskManager.API.Models.Data;
using TaskManager.Common.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TaskManager.API.Models.Services
{
    public class TasksService : AbstractService, ICommonService<TaskModel>
    {
        private readonly ApplicationContext _db;
        public TasksService(ApplicationContext db) => _db = db;
        public bool Create(TaskModel model)
        {
            return DoAction(() =>
            {
                var newTask = new TaskObject(model);
                _db.Tasks.Add(newTask);
                _db.SaveChanges();
            });
        }

        public TaskModel Get(int id)
        {
            return _db.Tasks.FirstOrDefault(t => t.Id == id)?.ToDto();
        }

        public bool Remove(int id)
        {
            return DoAction(() =>
            {
                var remTask = _db.Tasks.FirstOrDefault(t => t.Id == id);
                _db.Tasks.Remove(remTask);
                _db.SaveChanges();
            });
        }

        public bool Update(int id, TaskModel model)
        {
            return DoAction(() =>
            {
                var currentTask = _db.Tasks.FirstOrDefault(task => task.Id == id);

                if (currentTask.Name != model.Name && model.Name != null) currentTask.Name = model.Name;
                if (currentTask.Description != model.Description && model.Description != null) currentTask.Description = model.Description;
                if (currentTask.Image != model.Image && model.Image != null) currentTask.Image = model.Image;
                if (currentTask.StartDate != model.StartDate) currentTask.StartDate = model.StartDate;
                if (currentTask.EndDate != model.EndDate) currentTask.EndDate = model.EndDate;
                if (currentTask.File != model.File && model.File != null) currentTask.File = model.File;
                if (currentTask.ExecutorId != model.ExecutorId && model.ExecutorId != null) currentTask.ExecutorId = model.ExecutorId;

                _db.Tasks.Update(currentTask);
                _db.SaveChanges();
            });
        }

        public IQueryable<TaskModel> GetDeskTasks(int deskId)
        {
            return _db.Tasks.Where(t => t.DeskId == deskId).Select(t => t.ToShortDto());
        }

        public IQueryable<CommonModel> GetTasksForUser(int userId)
        {
            return _db.Tasks.Where(t => t.CreatorId == userId || t.ExecutorId == userId).Select(t => t.ToShortDto());
        }
    }

}
