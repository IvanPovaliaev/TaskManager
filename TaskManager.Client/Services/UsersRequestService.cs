using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private string _usersControllerUrl = $"{HOST}users/";

        private async Task<string> GetDataByUrl(HttpMethod method, string url, AuthToken token, string userName = null, string password = null)
        {
            var client = new HttpClient();
            HttpResponseMessage response = null;

            if(userName != null && password != null)
            {
                var authToken = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userName}:{password}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                if (method == HttpMethod.Get)
                    response = await client.GetAsync(url);
                if (method == HttpMethod.Post)
                    response = await client.PostAsync(url, new StringContent(authToken.ToString()));
            }
            else if (token != null)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
                if (method == HttpMethod.Get)
                    response = await client.GetAsync(url);
                if (method == HttpMethod.Post)
                    response = await client.PostAsync(url, new StringContent(token.access_token));
            }

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpStatusCode> SendDataByUrl(HttpMethod method, string url, AuthToken token, string data)
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
            var resultStr = GetDataByUrl(HttpMethod.Post, url, null, userName, password).Result;
            return JsonConvert.DeserializeObject<AuthToken>(resultStr);
        }

        public async Task<HttpStatusCode> CreateUser(AuthToken token, UserModel user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            return await SendDataByUrl(HttpMethod.Post, _usersControllerUrl, token, userJson);
        }

        public List<UserModel> GetAllUsers(AuthToken token)
        {
            var response = GetDataByUrl(HttpMethod.Get, _usersControllerUrl, token);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(response.Result);
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
    }
}
