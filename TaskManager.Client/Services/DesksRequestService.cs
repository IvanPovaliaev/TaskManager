using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class DesksRequestService : CommonRequestService
    {
        private string _desksControllerUrl = $"{HOST}desks/";

        public async Task<List<DeskModel>> GetDesksForCurrentUserAsync(AuthToken token)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, _desksControllerUrl, token);
            var desks = JsonConvert.DeserializeObject<List<DeskModel>>(response);
            return desks;
        }

        public async Task<DeskModel> GetDeskByIdAsync(AuthToken token, int deskId)
        {
            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_desksControllerUrl}{deskId}", token);
            var desk = JsonConvert.DeserializeObject<DeskModel>(response);
            return desk;
        }
        public async Task<List<DeskModel>> GetDesksByProjectAsync(AuthToken token, int projectId)
        {
            var parameters = new Dictionary<string, string>();
            parameters["projectId"] = projectId.ToString();

            var response = await GetDataByUrlAsync(HttpMethod.Get, $"{_desksControllerUrl}project", token, null, null, parameters);
            var desk = JsonConvert.DeserializeObject<List<DeskModel>>(response);
            return desk;
        }

        public async Task<HttpResponseMessage> CreateDeskAsync(AuthToken token, DeskModel desk)
        {
            var deskJson = JsonConvert.SerializeObject(desk);
            return await SendDataByUrlAsync(HttpMethod.Post, _desksControllerUrl, token, deskJson);
        }

        public async Task<HttpResponseMessage> UpdateDeskAsync(AuthToken token, DeskModel desk)
        {
            var deskJson = JsonConvert.SerializeObject(desk);
            return await SendDataByUrlAsync(HttpMethod.Patch, $"{_desksControllerUrl}{desk.Id}", token, deskJson);
        }

        public async Task<HttpStatusCode> DeleteDeskAsync(AuthToken token, int deskId)
        {
            return await DeleteDataByUrlAsync($"{_desksControllerUrl}{deskId}", token);
        }
    }
}
