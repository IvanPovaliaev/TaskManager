﻿using System;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public abstract class CommonRequestService
    {
        public const string HOST = "http://localhost:5128/api/";
        public async Task<string> GetDataByUrl(HttpMethod method, string url, AuthToken token, string userName = null, string password = null)
        {
            var client = new HttpClient();
            HttpResponseMessage response = null;

            if (userName != null && password != null)
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

        public async Task<HttpStatusCode> SendDataByUrl(HttpMethod method, string url, AuthToken token, string data)
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

        public async Task<HttpStatusCode> DeleteDataByUrl(string url, AuthToken token)
        {
            var result = new HttpResponseMessage();
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
            result = await client.DeleteAsync(url);

            return result.StatusCode;
        }
    }
}
