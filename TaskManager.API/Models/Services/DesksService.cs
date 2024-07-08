using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using TaskManager.API.Models.Abstractions;
using TaskManager.API.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.API.Models.Services
{
    public class DesksService : AbstractService, ICommonService<DeskModel>
    {
        private readonly ApplicationContext _db;
        public DesksService(ApplicationContext db) => _db = db;

        public bool Create(DeskModel model)
        {
            return DoAction(() =>
            {
                var newDesk = new Desk(model);
                _db.Desks.Add(newDesk);
                _db.SaveChanges();
            });
        }

        public DeskModel Get(int id)
        {
            var desk = _db.Desks.Include(d => d.Tasks).FirstOrDefault(d => d.Id == id);
            var deskModel = desk?.ToDto();
            if (deskModel == null)
                deskModel.TasksIds = desk?.Tasks.Select(t => t.Id).ToList();
            return deskModel;            
        }

        public bool Remove(int id)
        {
            return DoAction(() =>
            {
                var remDesk = _db.Desks.FirstOrDefault(d => d.Id == id);
                _db.Desks.Remove(remDesk);
                _db.SaveChanges();
            });
        }

        public bool Update(int id, DeskModel model)
        {
            return DoAction(() =>
            {
                var currentDesk = _db.Desks.FirstOrDefault(desk => desk.Id == id);

                if (currentDesk.Name != model.Name && model.Name != null) currentDesk.Name = model.Name;
                if (currentDesk.Description != model.Description && model.Description != null) currentDesk.Description = model.Description;
                if (currentDesk.Image != model.Image && model.Image != null) currentDesk.Image = model.Image;
                if (currentDesk.IsPrivate != model.IsPrivate) currentDesk.IsPrivate = model.IsPrivate;
                if (currentDesk.AuthorId != model.AuthorId && model.AuthorId != null) currentDesk.AuthorId = model.AuthorId;
                if (currentDesk.ProjectId != model.ProjectId) currentDesk.ProjectId = model.ProjectId;
                if (model.Columns != null) currentDesk.Columns = JsonConvert.SerializeObject(model.Columns);

                _db.Desks.Update(currentDesk);
                _db.SaveChanges();
            });
        }

        public IQueryable<CommonModel> GetAll(int userId)
        {
            return _db.Desks.Where(d => d.AuthorId == userId).Select(d => d.ToDto() as CommonModel);
        }

        public IQueryable<CommonModel> GetProjectDesks(int projectId, int userId) //возврашаются desks все неприватные desk + приватные desks текущего пользователя
        {
            return _db.Desks.Where(d => d.ProjectId == projectId &&
            ( d.AuthorId == userId || !d.IsPrivate)).Select(d => d.ToDto() as CommonModel);
        }
    }
}
