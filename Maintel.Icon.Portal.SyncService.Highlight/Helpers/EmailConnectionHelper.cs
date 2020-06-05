using Maintel.Icon.Portal.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using Maintel.Icon.Portal.SyncService.Highlight.Interfaces;
using Newtonsoft.Json;
using Maintel.Icon.Portal.Domain.Dtos;
using Maintel.Icon.Portal.Domain.Enums;

namespace Maintel.Icon.Portal.SyncService.Highlight.Helpers
{
    public class EmailConnectionHelper : IEmailHelper
    {

        private HttpClient _client = new HttpClient();
        private readonly string _communicationApi;
        public EmailConnectionHelper(string communicationApi)
        {
            _communicationApi = communicationApi;
        }

        private async Task<FullEmailListForRetrieveDto> retrieveEmailRequest()
        {
            FullEmailListForRetrieveDto emails = null;
            HttpResponseMessage response = await _client.GetAsync(
                $"api/email");
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                emails = await response.Content.ReadAsAsync<FullEmailListForRetrieveDto>();
            }
            return emails;
        }

        private async Task<bool> deleteEmailRequest(IEnumerable<string> list)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(list),Encoding.UTF8,"application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_communicationApi}api/email")
            };

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> postEmailRequest(EmailForMoveDto emailForMoveDto)
        {

            HttpResponseMessage response = await _client.PostAsync("api/email",
                                new StringContent(JsonConvert.SerializeObject(emailForMoveDto), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        public bool ConnectToEmailServer()
        {

            _client.BaseAddress = new Uri(_communicationApi);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return true;
        }

        public Task<FullEmailListForRetrieveDto> RetrieveEmailBatch(int batchSize)
        {
            return retrieveEmailRequest();
        }

        public bool DeleteBatch(IEnumerable<string> emailIds)
        {
            return deleteEmailRequest(emailIds).GetAwaiter().GetResult();
        }

        public bool MoveEmail(string id, CommunicationLocationEnum communicationLocationEnum)
        {
            return postEmailRequest(new EmailForMoveDto() { Id = id, Location = (int)communicationLocationEnum }).GetAwaiter().GetResult();
        }

    }
}
