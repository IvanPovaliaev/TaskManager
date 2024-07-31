using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TaskManager.API.Models.Data;
using TaskManager.API.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.API.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;
        public UsersController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)           
                return _usersService.Create(userModel) ? Ok() : NotFound();

            return BadRequest();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
                return _usersService.Update(id, userModel) ? Ok() : NotFound();

            return BadRequest();
        }

        [HttpGet("{id}")]
        public ActionResult<UserModel> GetUser(int id)
        {
            var user = _usersService.Get(id);
            return user == null ? NotFound() : Ok(_usersService.Get(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveUser(int id) => _usersService.Remove(id) ? Ok() : NotFound();

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
