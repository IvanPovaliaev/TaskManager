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

namespace TaskManager.Client.Services
{
    public class UsersRequestService
    {
        private const string HOST = "http://localhost:5128/api/";

        private async Task<string> GetDataByUrl(string url, string userName, string password)
        {
            var client = new HttpClient();
            var authToken = Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userName}:{password}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            var response = await client.PostAsync(url, new StringContent(authToken.ToString()));
            
            return await response.Content.ReadAsStringAsync();

        }
        public AuthToken GetToken(string userName, string password)
        {
            var url = $"{HOST}account/token";
            var resultStr = GetDataByUrl(url, userName, password).Result;
            return JsonConvert.DeserializeObject<AuthToken>(resultStr);
        }
    }
}
