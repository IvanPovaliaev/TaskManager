using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.API.Models.Data;
using TaskManager.API.Models.Services;
using TaskManager.Common.Models;
using TaskManager.Common.Models.Services;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DesksController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly DesksService _desksService;
        private readonly ValidationService _validationService;

        public DesksController(ApplicationContext db)
        {
            _usersService = new UsersService(db);
            _desksService = new DesksService(db);
            _validationService = new ValidationService();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommonModel>>> GetDesksForCurrentUser()
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Array.Empty<CommonModel>();

            var result = await _desksService.GetAll(user.Id).ToListAsync();
            return result is null ? NoContent() : Ok(result);           
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var desk = _desksService.Get(id);
            return desk is null ? NotFound() : Ok(desk);
        }

        [HttpGet("project")]
        public async Task<ActionResult<IEnumerable<CommonModel>>> GetProjectDesk(int projectId)
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);
            if (user == null) return Array.Empty<CommonModel>();

            var result = await _desksService.GetProjectDesks(projectId, user.Id).ToListAsync();
            return result is null ? NoContent() : Ok(result);          
        }

        [HttpPost]
        public IActionResult Create([FromBody] DeskModel model)
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Unauthorized();
            if (model == null) return BadRequest();

            var isCorrectInputData = _validationService.IsCorrectDeskInputData(model, out var messages);
            if (!isCorrectInputData) return BadRequest(messages);

            model.AuthorId = user.Id;

            var result = _desksService.Create(model);
            return result ? Ok() : NotFound();
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, [FromBody] DeskModel deskModel)
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);

            if (user == null) return Unauthorized();
            if (deskModel == null) return BadRequest();

            var isCorrectInputData = _validationService.IsCorrectDeskInputData(deskModel, out var messages);
            if (!isCorrectInputData) return BadRequest(messages);

            var result = _desksService.Update(id, deskModel);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id) => _desksService.Remove(id) ? Ok() : NotFound();
    }
}
