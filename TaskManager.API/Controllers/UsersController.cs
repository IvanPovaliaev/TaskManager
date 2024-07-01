using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UserService _userService;
        public UsersController(ApplicationContext db)
        {
            _db = db;
            _userService = new UserService(db);
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult TestApi() => Ok("Hello Word!");
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)           
                return _userService.Create(userModel) ? Ok() : NotFound();

            return BadRequest();
        }
        [HttpPatch("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
                return _userService.Update(id, userModel) ? Ok() : NotFound();

            return BadRequest();
        }
        [HttpDelete("{id}")]
        public IActionResult RemoveUser(int id) => _userService.Remove(id) ? Ok() : NotFound();
        [HttpGet()]
        public async Task<IEnumerable<UserModel>> GetUsers() => await _db.Users.Select(u => u.ToDto()).ToListAsync();
        [HttpPost("all")]
        public async Task<IActionResult> CreateMultipleUsers([FromBody] IEnumerable<UserModel> usersModels)
        {
            if (!usersModels.IsNullOrEmpty())
                return _userService.CreateMultipleUsers(usersModels) ? Ok() : NotFound();

            return BadRequest();
        }
    }
}
