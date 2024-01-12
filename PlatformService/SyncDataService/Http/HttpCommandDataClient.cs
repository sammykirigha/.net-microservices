using PlatformService.Dtos;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataService.Http
{
    public class HttpCommandDataClient: ICommandDataClient
    {
        private HttpClient _httpClient;
        private IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatFormReadDto platFormReadDto)
        {
            var httpCont = new StringContent(JsonSerializer.Serialize(platFormReadDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/commands/platforms", httpCont);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("===> Sync post to commandservice was ok");
            }else
            {
                Console.WriteLine("===> Sync post to commandservice was never ok");
            }
        }
    }
}