using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TaskManager.API.Models.Data;
using TaskManager.API.Models.Services;
using System.IdentityModel.Tokens.Jwt;
using TaskManager.API.Models;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _userService;

        public AccountController(ApplicationContext db)
        {
            _db = db;
            _userService = new UsersService(db);
        }

        [Authorize]
        [HttpGet("info")]
        public IActionResult GetCurrentUserInfo()
        {
            string username = HttpContext.User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.Email == username);

            return user is null ? NotFound() : Ok(user.ToDto());
        }
        [HttpPost("token")]
        public IActionResult GetToken()
        {
            var userData = _userService.GetUserLoginPassFromBasicAuth(Request);
            var login = userData.Item1;
            var pass = userData.Item2;
            var identity = _userService.GetIdentity(login, pass);

            if (identity is null) return NotFound("User not found");

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Ok(response);
        }
        //[Authorize]
        //[HttpPatch("update")]
        //public IActionResult UpdateUser([FromBody] UserModel userModel)
        //{
        //    if (userModel != null)
        //    {
        //        var userName = HttpContext.User.Identity.Name;

        //        var user = _db.Users.FirstOrDefault(u => u.Email == userName);

        //        if (user != null)
        //        {
        //            user.FirstName = userModel.FirstName;
        //            user.Surname = userModel.Surname;
        //            user.Password = userModel.Password;
        //            user.Phone = userModel.Phone;
        //            user.Photo = userModel.Photo;

        //            _db.Users.Update(user);
        //            _db.SaveChanges();
        //            return Ok();
        //        }
        //        return NotFound();
        //    }
        //    return BadRequest();
        //}
    }
}
