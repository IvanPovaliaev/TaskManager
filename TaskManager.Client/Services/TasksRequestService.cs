using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class TasksRequestService : CommonRequestService
    {
        private string _tasksControllerUrl = $"{HOST}tasks/";

        public async Task<List<TaskModel>> GetTasksForCurrentUser(AuthToken token)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{_tasksControllerUrl}user", token);
            var tasks = JsonConvert.DeserializeObject<List<TaskModel>>(response);
            return tasks;
        }
        public async Task<TaskModel> GetTaskById(AuthToken token, int taskId)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{_tasksControllerUrl}{taskId}", token);
            var task = JsonConvert.DeserializeObject<TaskModel>(response);
            return task;
        }

        public async Task<List<TaskModel>> GetTasksByDesk(AuthToken token, int deskId)
        {
            var parameters = new Dictionary<string, string>();
            parameters["deskId"] = deskId.ToString();

            var response = await GetDataByUrl(HttpMethod.Get, $"{_tasksControllerUrl}", token, null, null, parameters);
            var task = JsonConvert.DeserializeObject<List<TaskModel>>(response);
            return task;
        }

        public async Task<HttpStatusCode> CreateTask(AuthToken token, TaskModel task)
        {
            var taskJson = JsonConvert.SerializeObject(task);
            return await SendDataByUrl(HttpMethod.Post, _tasksControllerUrl, token, taskJson);
        }

        public async Task<HttpStatusCode> UpdateTask(AuthToken token, TaskModel task)
        {
            var taskJson = JsonConvert.SerializeObject(task);
            return await SendDataByUrl(HttpMethod.Patch, $"{_tasksControllerUrl}{task.Id}", token, taskJson);
        }

        public async Task<HttpStatusCode> DeleteTaskById(AuthToken token, int taskId)
        {
            return await DeleteDataByUrl($"{_tasksControllerUrl}{taskId}", token);
        }
    }
}
