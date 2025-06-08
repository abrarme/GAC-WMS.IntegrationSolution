using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GAC_WMS.IntegrationSolution.Clients
{
    
    public class WmsClient : IWmsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WmsClient> _logger;
        private readonly RetryPolicyHandler _retryPolicyHandler;
        private readonly IHttpClientFactory _httpClientFactory;

        public WmsClient(HttpClient httpClient, ILogger<WmsClient> logger, RetryPolicyHandler retryPolicyHandler, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _logger = logger;
            _retryPolicyHandler = retryPolicyHandler;
            _httpClientFactory = httpClientFactory;
        }

        public async Task PushDataAsync(IEnumerable<dynamic> list, string endPoint)
        {
            try
            {

                var json = JsonSerializer.Serialize(list, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                _logger.LogInformation("Sending payload to WMS:\n{Json}", json);

                var Requestcontent = new StringContent(json);
                Requestcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.PostAsync(endPoint, Requestcontent);
      
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to push products. Status: {StatusCode}, Response: {Content}", response.StatusCode, content);
                    throw new HttpRequestException($"WMS API push failed: {response.StatusCode}");
                }

                _logger.LogInformation("Successfully pushed {Count} products to WMS.", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while pushing products to WMS.");
                throw;
            }
        }

       
    }

}
