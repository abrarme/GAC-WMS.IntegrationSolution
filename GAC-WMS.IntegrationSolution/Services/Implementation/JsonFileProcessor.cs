using GAC_WMS.IntegrationSolution.Clients;
using GAC_WMS.IntegrationSolution.Helper;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Services.Interface;
using System.Dynamic;
using System.Text.Json;

namespace GAC_WMS.IntegrationSolution.Services.Implementation
{
    public class JsonFileProcessor : IFileProcessor
    {
        private readonly IWmsClient _wmsClient;
        private readonly ILogger<JsonFileProcessor> _logger;

        public JsonFileProcessor(IWmsClient wmsClient, ILogger<JsonFileProcessor> logger)
        {
            _wmsClient = wmsClient;
            _logger = logger;
        }

        public async Task ProcessAsync(string filePath, string endPoint)
        {
            try
            {

                var json = await File.ReadAllTextAsync(filePath);
                var records = JsonSerializer.Deserialize<List<ExpandoObject>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (records != null && records.Any())
                {
                    await _wmsClient.PushDataAsync(records, endPoint);
                    FileHelper.Archive(filePath);
                }
                //var json = await File.ReadAllTextAsync(filePath);
                //var products = JsonSerializer.Deserialize<List<Product>>(json);
                //await _wmsClient.PushDataAsync(products, endPoint);
                //FileHelper.Archive(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process JSON file: {FilePath}", filePath);
                FileHelper.MoveToError(filePath, ex);
            }
        }
    }
}
