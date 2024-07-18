using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class ProjectsRequestService : CommonRequestService
    {
        private string _projectsControllerUrl = $"{HOST}projects/";
        public async Task<List<ProjectModel>> GetAllProjects(AuthToken token)
        {
            var response = await GetDataByUrl(HttpMethod.Get, _projectsControllerUrl, token);
            var projects = JsonConvert.DeserializeObject<List<ProjectModel>>(response);
            return projects;
        }

        public async Task<ProjectModel> GetProjectById(AuthToken token, int projectId)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{_projectsControllerUrl}{projectId}", token);
            var project = JsonConvert.DeserializeObject<ProjectModel>(response);
            return project;
        }

        public async Task<HttpStatusCode> CreateProject(AuthToken token, ProjectModel project)
        {
            var projectJson = JsonConvert.SerializeObject(project);
            return await SendDataByUrl(HttpMethod.Post, _projectsControllerUrl, token, projectJson);
        }

        public async Task<HttpStatusCode> UpdateProject(AuthToken token, ProjectModel project)
        {
            var deskJson = JsonConvert.SerializeObject(project);
            return await SendDataByUrl(HttpMethod.Patch, $"{_projectsControllerUrl}{project.Id}", token, deskJson);
        }

        public async Task<HttpStatusCode> DeleteProject(AuthToken token, int projectId)
        {
            return await DeleteDataByUrl($"{_projectsControllerUrl}{projectId}", token);
        }

        public async Task<HttpStatusCode> AddUsersToProject(AuthToken token, int projectId, IEnumerable<int> usersIds)
        {
            var usersUdsJson = JsonConvert.SerializeObject(usersIds);
            return await SendDataByUrl(HttpMethod.Patch, $"{_projectsControllerUrl}{projectId}/users", token, usersUdsJson);
        }
        public async Task<HttpStatusCode> RemoveUsersFromProject(AuthToken token, int projectId, IEnumerable<int> usersIds)
        {
            var usersUdsJson = JsonConvert.SerializeObject(usersIds);
            return await SendDataByUrl(HttpMethod.Patch, $"{_projectsControllerUrl}{projectId}/users/remove", token, usersUdsJson);
        }
    }
}
