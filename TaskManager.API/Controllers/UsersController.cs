using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;
        private readonly ValidationService _validationService;

        public UsersController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
            _validationService = new ValidationService();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] UserModel userModel)
        {
            if (_db.Users.Any(u => u.Email == userModel.Email))
                return BadRequest("User with this email already exist!");

            if (userModel != null)
            {
                var isCorrectInputData = _validationService.IsCorrectUserInputData(userModel, out var messages);

                if (!isCorrectInputData) return BadRequest(messages);

                return _usersService.Create(userModel) ? Ok() : NotFound();
            }                

            return BadRequest();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                var isCorrectInputData = _validationService.IsCorrectUserInputData(userModel, out var messages);

                if (!isCorrectInputData) return BadRequest(messages);

                return _usersService.Update(id, userModel) ? Ok() : NotFound();
            }               

            return BadRequest();
        }

        [HttpGet("{id}")]
        public ActionResult<UserModel> Get(int id)
        {
            var user = _usersService.Get(id);
            return user == null ? NotFound() : Ok(_usersService.Get(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Remove(int id) => _usersService.Remove(id) ? Ok() : NotFound();

        [HttpGet()]
        public async Task<IEnumerable<UserModel>> GetUsers() => await _db.Users.Select(u => u.ToDto()).ToListAsync();

        [HttpPost("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateMultipleUsers([FromBody] IEnumerable<UserModel> usersModels)
        {
            if (!usersModels.IsNullOrEmpty())
                return _usersService.CreateMultipleUsers(usersModels) ? Ok() : NotFound();

            return BadRequest();
        }

        [HttpGet("{id}/admin")]
        public ActionResult<int?> GetProjectAdminId(int id)
        {
            var admin = _usersService.GetProjectAdmin(id);
            return admin == null ? NotFound(null) : Ok(admin.Id);
        }
    }
}
