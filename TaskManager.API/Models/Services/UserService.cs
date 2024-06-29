using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TaskManager.API.Models.Data;

namespace TaskManager.API.Models.Services
{
    public class UserService
    {
        private readonly ApplicationContext _db;
        public UserService(ApplicationContext db) => _db = db;

        public Tuple<string, string> GetUserLoginPassFromBasicAuth(HttpRequest request)
        {
            var userName = "";
            var userPass = "";
            var authHeader = request.Headers["Authorization"].ToString();
            
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                var encodedUserNamepass = authHeader.Replace("Basic ", "");
                var encoding = Encoding.GetEncoding("iso-8859-1");

                var namePassArray = encoding.GetString(Convert.FromBase64String(encodedUserNamepass)).Split(':');
                userName = namePassArray[0];
                userPass = namePassArray[1];
            }
            return new Tuple<string, string>(userName, userPass);
        }

        public User GetUser(string login, string password)
        {
            return _db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        }

        public ClaimsIdentity GetIdentity(string username, string password)
        {
            var currentUser = GetUser(username, password);
            if (currentUser != null)
            {
                currentUser.LastLoginDay = DateTime.Now;
                _db.Users.Update(currentUser);
                _db.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, currentUser.Role.ToString())
                };

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }
    }
}
