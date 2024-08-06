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

        public async Task<AuthToken> GetTokenAsync(string userName, string password)
        {
            var url = $"{HOST}account/token";
            var resultStr = await GetDataByUrlAsync(HttpMethod.Post, url, null, userName, password);
            try
            {
                return JsonConvert.DeserializeObject<AuthToken>(resultStr);
            }
            catch (JsonReaderException ex)
            {
                return null;
            }
        }

        public async Task<UserModel> GetCurrentUserAsync(AuthToken token)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{HOST}account/info", token);
            var user = JsonConvert.DeserializeObject<UserModel>(response);
            return user;
        }

        public async Task<UserModel> GetUserByIdAsync(AuthToken token, int userId)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_usersControllerUrl}{userId}", token);
            try
            {
                var user = JsonConvert.DeserializeObject<UserModel>(response);
                return user;
            }
            catch (JsonReaderException ex)
            {
                return new UserModel();
            }                  
        }

        public async Task<HttpResponseMessage> CreateUserAsync(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrlAsync(HttpMethod.Post, _usersControllerUrl, token, userJson);
        }

        public async Task<List<UserModel>> GetAllUsersAsync(AuthToken token)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, _usersControllerUrl, token);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(response);
            return users;
        }

        public async Task<HttpStatusCode> DeleteUserAsync(AuthToken token, int userId)
        {
            return await DeleteDataByUrlAsync($"{_usersControllerUrl}{userId}", token);
        }

        public async Task<HttpResponseMessage> CreateMultipleUsersAsync(AuthToken token, List<UserModel> users)
        {
            var userJson = JsonConvert.SerializeObject(users);
            return await SendDataByUrlAsync(HttpMethod.Post, $"{_usersControllerUrl}all", token, userJson);
        }

        public async Task<HttpResponseMessage> UpdateUserAsync(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrlAsync(HttpMethod.Patch, $"{_usersControllerUrl}{user.Id}", token, userJson);
        }

        public async Task<int?> GetProjectUserAdminAsync(AuthToken token, int userId)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_usersControllerUrl}{userId}/admin", token);
            return int.TryParse(response, out var result) ? result : null;
        }
    }
}
