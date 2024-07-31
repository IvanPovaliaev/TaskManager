using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class UsersRequestService : CommonRequestService
    {
        private string _usersControllerUrl = $"{HOST}users/";

        public async Task<AuthToken> GetToken(string userName, string password)
        {
            var url = $"{HOST}account/token";
            var resultStr = await GetDataByUrl(HttpMethod.Post, url, null, userName, password);
            return JsonConvert.DeserializeObject<AuthToken>(resultStr);
        }

        public async Task<UserModel> GetCurrentUser(AuthToken token)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{HOST}account/info", token);
            var user = JsonConvert.DeserializeObject<UserModel>(response);
            return user;
        }

        public async Task<UserModel> GetUserById(AuthToken token, int userId)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{_usersControllerUrl}{userId}", token);
            var user = JsonConvert.DeserializeObject<UserModel>(response);
            return user;
        }

        public async Task<HttpStatusCode> CreateUser(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrl(HttpMethod.Post, _usersControllerUrl, token, userJson);
        }

        public async Task<List<UserModel>> GetAllUsers(AuthToken token)
        {
            var response = await GetDataByUrl(HttpMethod.Get, _usersControllerUrl, token);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(response);
            return users;
        }

        public async Task<HttpStatusCode> DeleteUser(AuthToken token, int userId)
        {
            return await DeleteDataByUrl($"{_usersControllerUrl}{userId}", token);
        }

        public async Task<HttpStatusCode> CreateMultipleUsers(AuthToken token, List<UserModel> users)
        {
            var userJson = JsonConvert.SerializeObject(users);
            return await SendDataByUrl(HttpMethod.Post, $"{_usersControllerUrl}all", token, userJson);
        }

        public async Task<HttpStatusCode> UpdateUser(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrl(HttpMethod.Patch, $"{_usersControllerUrl}{user.Id}", token, userJson);
        }

        public async Task<int?> GetProjectUserAdmin(AuthToken token, int userId)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{_usersControllerUrl}{userId}/admin", token);
            return int.TryParse(response, out var result) ? result : null;
        }
    }
}
