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
using TaskManager.Common.Models;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;
        public UsersController(ApplicationContext db) => _db = db;

        [HttpGet("test")]
        public IActionResult TestApi() => Ok("Hello Word!");

        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                var newUser = new User(userModel.FirstName, userModel.Surname, userModel.Email,
                    userModel.Password, userModel.Role, userModel.Phone, userModel.Photo);
                _db.Users.Add(newUser);
                _db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        [HttpPatch("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == id);

                if (user != null)
                {
                    user.FirstName = userModel.FirstName;
                    user.Surname = userModel.Surname;
                    user.Password = userModel.Password;
                    user.Phone = userModel.Phone;
                    user.Photo = userModel.Photo;
                    user.Role = userModel.Role;
                    user.Email = userModel.Email;
                    _db.Users.Update(user);
                    _db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }
        [HttpDelete("remove/{id}")]
        public IActionResult RemoveUser(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Ok();
            }
            return NotFound();            
        }
        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            return await _db.Users.Select(u => u.ToDto()).ToListAsync();
        }

        [HttpPost("create/all")]
        public async Task<IActionResult> CreateMultipleUsers([FromBody] IEnumerable<UserModel> usersModels)
        {
            if (!usersModels.IsNullOrEmpty())
            {
                var newUsers = usersModels.Select(userM => new User(userM));
                _db.Users.AddRange(newUsers);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}
