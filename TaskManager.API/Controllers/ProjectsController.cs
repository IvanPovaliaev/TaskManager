using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.API.Models;
using TaskManager.API.Models.Data;
using TaskManager.API.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;
        private readonly ProjectsService _projectService;
        public ProjectsController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
            _projectService = new ProjectsService(db);
        }

        [HttpGet]
        public async Task<IEnumerable<ProjectModel>> Get() => await _db.Projects.Select(p => p.ToDto()).ToListAsync();

        [HttpPost]
        public IActionResult Create([FromBody] ProjectModel projectModel)
        {
            if (projectModel == null) return BadRequest();

            var user = _usersService.GetUser(HttpContext.User.Identity.Name);
            if (user != null)
            {
                var admin = _db.ProjectAdmins.FirstOrDefault(a => a.Id == user.Id);
                if (admin == null)
                {
                    admin = new ProjectAdmin(user);
                    _db.ProjectAdmins.Add(admin);
                }
                projectModel.AdminId = admin.Id;
            }
            return _projectService.Create(projectModel) ? Ok() : NotFound();                                  
        }

        [HttpPatch]
        public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
                return _projectService.Update(id, projectModel) ? Ok() : NotFound();

            return BadRequest();
        }

        [HttpDelete]
        public IActionResult Remove(int id) => _projectService.Remove(id) ? Ok() : NotFound();
    }
}
