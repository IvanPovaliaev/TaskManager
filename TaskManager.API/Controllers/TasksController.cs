using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class TasksController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly TasksService _tasksService;

        public TasksController(ApplicationContext db)
        {
            _usersService = new UsersService(db);
            _tasksService = new TasksService(db);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasksByDesk(int deskId)
        {
            var result = await _tasksService.GetDeskTasks(deskId).ToListAsync();
            return result is null ? NoContent() : Ok(result);
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasksForCurrentUser()
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Unauthorized(Array.Empty<CommonModel>());

            var result = await _tasksService.GetTasksForUser(user.Id).ToListAsync();
            return result is null ? NoContent() : Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var task = _tasksService.Get(id);
            return task is null ? NotFound() : Ok(task);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TaskModel model)
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Unauthorized();
            if (model == null) return BadRequest();

            model.CreatorId = user.Id;

            var result = _tasksService.Create(model);            
            return result ? Ok() : NotFound();
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, [FromBody] TaskModel model)
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Unauthorized();
            if (model == null) return BadRequest();

            var result = _tasksService.Update(id, model);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id) => _tasksService.Remove(id) ? Ok() : NotFound();
    }
}
