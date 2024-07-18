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

        public async Task<List<DeskModel>> GetDesksForCurrentUser(AuthToken token)
        {
            var response = await GetDataByUrl(HttpMethod.Get, _desksControllerUrl, token);
            var desks = JsonConvert.DeserializeObject<List<DeskModel>>(response);
            return desks;
        }

        public async Task<DeskModel> GetDeskById(AuthToken token, int deskId)
        {
            var response = await GetDataByUrl(HttpMethod.Get, $"{_desksControllerUrl}{deskId}", token);
            var desk = JsonConvert.DeserializeObject<DeskModel>(response);
            return desk;
        }
        public async Task<List<DeskModel>> GetDesksByProject(AuthToken token, int projectId)
        {
            var parameters = new Dictionary<string, string>();
            parameters["projectId"] = projectId.ToString();

            var response = await GetDataByUrl(HttpMethod.Get, $"{_desksControllerUrl}project", token, null, null, parameters);
            var desk = JsonConvert.DeserializeObject<List<DeskModel>>(response);
            return desk;
        }

        public async Task<HttpStatusCode> CreateDesk(AuthToken token, DeskModel desk)
        {
            var deskJson = JsonConvert.SerializeObject(desk);
            return await SendDataByUrl(HttpMethod.Post, _desksControllerUrl, token, deskJson);
        }

        public async Task<HttpStatusCode> UpdateDesk(AuthToken token, DeskModel desk)
        {
            var deskJson = JsonConvert.SerializeObject(desk);
            return await SendDataByUrl(HttpMethod.Patch, $"{_desksControllerUrl}{desk.Id}", token, deskJson);
        }

        public async Task<HttpStatusCode> DeleteDeskById(AuthToken token, int deskId)
        {
            return await DeleteDataByUrl($"{_desksControllerUrl}{deskId}", token);
        }
    }
}
