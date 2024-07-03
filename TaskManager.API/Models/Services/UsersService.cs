using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TaskManager.API.Models.Abstractions;
using TaskManager.API.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.API.Models.Services
{
    public class UsersService : AbstractService, ICommonService<UserModel>
    {
        private readonly ApplicationContext _db;
        public UsersService(ApplicationContext db) => _db = db;

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

        public User GetUser(string login) => _db.Users.FirstOrDefault(u => u.Email == login);

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

        public bool Create(UserModel model)
        {
            return DoAction(() =>
            {
                var newUser = new User(model.FirstName, model.Surname, model.Email,
                    model.Password, model.Role, model.Phone, model.Photo);
                _db.Users.Add(newUser);
                _db.SaveChanges();
            });         
        }

        public bool Update(int id, UserModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                return DoAction(() =>
                {
                    user.FirstName = model.FirstName;
                    user.Surname = model.Surname;
                    user.Password = model.Password;
                    user.Phone = model.Phone;
                    user.Photo = model.Photo;
                    user.Role = model.Role;
                    user.Email = model.Email;
                    _db.Users.Update(user);
                    _db.SaveChanges();
                });
            }
            return false;
        }

        public bool Remove(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {

            }
            return false;
        }

        public bool CreateMultipleUsers(IEnumerable<UserModel> usersModels)
        {
            return DoAction(() =>
            {
                var newUsers = usersModels.Select(userM => new User(userM));
                _db.Users.AddRange(newUsers);
                _db.SaveChangesAsync();                
            });
        }

        public UserModel Get(int id) => _db.Users.FirstOrDefault(u => u.Id == id).ToDto();

        public IEnumerable<UserModel> GetAllByIds(IEnumerable<int> usersIds)
        {
            foreach (var id in usersIds)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == id).ToDto();
                yield return user;
            }
        }
    }
}
