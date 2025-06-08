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

               // Dynamically specify the type (e.g. Product, Customer, or ExpandoObject)
                var records = await DeserializeDynamicJsonListAsync<ExpandoObject>(filePath);

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

        public async Task<List<T>?> DeserializeDynamicJsonListAsync<T>(string filePath, string? rootPropertyName = null)
        {
            var json = await File.ReadAllTextAsync(filePath);

            using var document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            // If rootPropertyName not provided, try to detect first property with array value
            if (rootPropertyName == null)
            {
                foreach (var property in root.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        rootPropertyName = property.Name;
                        break;
                    }
                }

                if (rootPropertyName == null)
                {
                    throw new InvalidOperationException("No array property found at the root of JSON.");
                }
            }

            if (!root.TryGetProperty(rootPropertyName, out var arrayElement))
                throw new InvalidOperationException($"Root JSON does not contain property '{rootPropertyName}'.");

            var arrayJson = arrayElement.GetRawText();

            return JsonSerializer.Deserialize<List<T>>(arrayJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

    }
}
