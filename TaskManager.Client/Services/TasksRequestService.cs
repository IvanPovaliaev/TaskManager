using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class TasksRequestService : CommonRequestService
    {
        private string _tasksControllerUrl = $"{HOST}tasks/";

        public async Task<List<TaskModel>> GetTasksForCurrentUserAsync(AuthToken token)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_tasksControllerUrl}user", token);
            return JsonConvert.DeserializeObject<List<TaskModel>>(response);
        }
        public async Task<TaskModel> GetTaskByIdAsync(AuthToken token, int taskId)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_tasksControllerUrl}{taskId}", token);
            return JsonConvert.DeserializeObject<TaskModel>(response);
        }

        public async Task<List<TaskModel>> GetTasksByDeskAsync(AuthToken token, int deskId)
        {
            var parameters = new Dictionary<string, string>();
            parameters["deskId"] = deskId.ToString();

            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_tasksControllerUrl}", token, null, null, parameters);
            return JsonConvert.DeserializeObject<List<TaskModel>>(response);
        }

        public async Task<HttpResponseMessage> CreateTaskAsync(AuthToken token, TaskModel task)
        {
            var taskJson = JsonConvert.SerializeObject(task);
            return await SendDataByUrlAsync(HttpMethod.Post, _tasksControllerUrl, token, taskJson);
        }

        public async Task<HttpResponseMessage> UpdateTaskAsync(AuthToken token, TaskModel task)
        {
            var taskJson = JsonConvert.SerializeObject(task);
            return await SendDataByUrlAsync(HttpMethod.Patch, $"{_tasksControllerUrl}{task.Id}", token, taskJson);
        }

        public async Task<HttpStatusCode> DeleteTaskAsync(AuthToken token, int taskId)
        {
            return await DeleteDataByUrlAsync($"{_tasksControllerUrl}{taskId}", token);
        }
    }
}
