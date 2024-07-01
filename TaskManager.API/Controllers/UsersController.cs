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

namespace TaskManager.API.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)           
                return _usersService.Create(userModel) ? Ok() : NotFound();

            return BadRequest();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
                return _usersService.Update(id, userModel) ? Ok() : NotFound();

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveUser(int id) => _usersService.Remove(id) ? Ok() : NotFound();

        [HttpGet()]
        public async Task<IEnumerable<UserModel>> GetUsers() => await _db.Users.Select(u => u.ToDto()).ToListAsync();

        [HttpPost("all")]
        public async Task<IActionResult> CreateMultipleUsers([FromBody] IEnumerable<UserModel> usersModels)
        {
            if (!usersModels.IsNullOrEmpty())
                return _usersService.CreateMultipleUsers(usersModels) ? Ok() : NotFound();

            return BadRequest();
        }
    }
}
