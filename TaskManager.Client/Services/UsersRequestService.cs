using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class UsersRequestService
    {
        private const string HOST = "http://localhost:5128/api/";
        private string _userController = $"{HOST}/users/";

        private async Task<string> GetDataByUrl(string url, string userName = null, string password = null)
        {
            var client = new HttpClient();
            var authToken = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userName}:{password}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            var response = await client.PostAsync(url, new StringContent(authToken.ToString()));
            
            return await response.Content.ReadAsStringAsync();

        }

        private async Task<HttpStatusCode> SendDataByUrl(HttpMethod method, string url, AuthToken token, string data = null)
        {
            var result = new HttpResponseMessage();
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);           

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            if (method == HttpMethod.Post)
                result = await client.PostAsync(url, content);

            if (method == HttpMethod.Patch)
                result = await client.PatchAsync(url, content);

            return result.StatusCode;
        }

        private async Task<HttpStatusCode> DeleteDataByUrl(string url, AuthToken token)
        {
            var result = new HttpResponseMessage();
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
            result = await client.DeleteAsync(url);

            return result.StatusCode;
        }

        public AuthToken GetToken(string userName, string password)
        {
            var url = $"{HOST}account/token";
            var resultStr = GetDataByUrl(url, userName, password).Result;
            return JsonConvert.DeserializeObject<AuthToken>(resultStr);
        }

        public async Task<HttpStatusCode> CreateUser(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrl(HttpMethod.Post, _userController, token, userJson);
        }

        public List<UserModel> GetAllUsers(AuthToken token)
        {
            var response = GetDataByUrl(_userController);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(response.Result);
            return users;
        }

        public async Task<HttpStatusCode> DeleteUser(AuthToken token, int userId)
        {
            return await DeleteDataByUrl($"{_userController}userId", token);
        }

        public async Task<HttpStatusCode> CreateMultipleUser(AuthToken token, List<UserModel> users)
        {
            var userJson = JsonConvert.SerializeObject(users);
            return await SendDataByUrl(HttpMethod.Post, $"{_userController}all", token, userJson);
        }

        public async Task<HttpStatusCode> UpdateUser(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrl(HttpMethod.Patch, $"{_userController}{user.Id}", token, userJson);
        }
    }
}
