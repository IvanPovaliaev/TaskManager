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
using TaskManager.Common.Models.Services;

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
        private readonly ValidationService _validationService;
        public ProjectsController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
            _projectService = new ProjectsService(db);
            _validationService = new ValidationService();
        }

        [HttpGet]
        public async Task<IEnumerable<CommonModel>> Get()
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);
            if (user.Role != UserRole.Admin) return await _projectService.GetByUserId(user.Id);
            return await _projectService.GetAll().ToListAsync();            
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var project = _projectService.Get(id);
            return project == null ? NotFound() : Ok(project);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProjectModel projectModel)
        {
            if (projectModel == null) return BadRequest();

            var isCorrectInputData = _validationService.IsCorrectProjectInputData(projectModel, out var messages);

            if (!isCorrectInputData) return BadRequest(messages);

            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Unauthorized();


            if (user.Role == UserRole.Admin || user.Role == UserRole.Editor)
            {
                var admin = _db.ProjectAdmins.FirstOrDefault(a => a.UserId == user.Id);
                if (admin == null)
                {
                    admin = new ProjectAdmin(user);
                    _db.ProjectAdmins.Add(admin);
                    _db.SaveChanges();
                }
                projectModel.AdminId = admin.Id;
            }
            return _projectService.Create(projectModel) ? Ok() : NotFound();            
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                var isCorrectInputData = _validationService.IsCorrectProjectInputData(projectModel, out var messages);

                if (!isCorrectInputData) return BadRequest(messages);

                var user = _usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Editor)
                    {
                        return _projectService.Update(id, projectModel) ? Ok() : NotFound();
                    }
                    return Unauthorized();
                }
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id) => _projectService.Remove(id) ? Ok() : NotFound();

        [HttpPatch("{id}/users")]
        public IActionResult AddUsersToProjects(int id, [FromBody] IEnumerable<int> usersIds)
        {
            if (usersIds != null)
            {
                var user = _usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Editor)
                    {
                        _projectService.AddUsersToProject(id, usersIds);
                        return Ok();
                    }
                    return Unauthorized();
                }
            }

            return BadRequest();
        }

        [HttpPatch("{id}/users/remove")]
        public IActionResult RemoveUsersFromProjects(int id, [FromBody] IEnumerable<int> usersIds)
        {
            if (usersIds != null)
            {
                var user = _usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Editor)
                    {
                        _projectService.RemoveUsersFromProject(id, usersIds);
                        return Ok();
                    }
                    return Unauthorized();
                }
            }

            return BadRequest();
        }

    }
}
