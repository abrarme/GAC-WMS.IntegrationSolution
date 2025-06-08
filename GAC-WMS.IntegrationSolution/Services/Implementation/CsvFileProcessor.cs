using CsvHelper;
using CsvHelper.Configuration;
using GAC_WMS.IntegrationSolution.Clients;
using GAC_WMS.IntegrationSolution.Helper;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Services.Interface;
using System.Globalization;

namespace GAC_WMS.IntegrationSolution.Services.Implementation
{
    public class CsvFileProcessor : IFileProcessor
    {
        private readonly IWmsClient _wmsClient;
        private readonly ILogger<CsvFileProcessor> _logger;

        public CsvFileProcessor(IWmsClient wmsClient, ILogger<CsvFileProcessor> logger)
        {
            _wmsClient = wmsClient;
            _logger = logger;
        }

        public async Task ProcessAsync(string filePath, string endPoint)
        {
            try
            {
                _logger.LogInformation("Processing CSV file: {FilePath}", filePath);

                var lines = await File.ReadAllLinesAsync(filePath);
                if (lines.Length <= 1)
                {
                    _logger.LogWarning("File {FilePath} has no content.", filePath);
                    return;
                }

                using (var reader = new StreamReader(filePath))
                {
                    using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null
                    });
                    var records = csv.GetRecords<dynamic>().ToList();

                    if (records.Any())
                    {
                        await _wmsClient.PushDataAsync(records, endPoint);

                      
                    }
                        
                }

                FileHelper.Archive(filePath);
                _logger.LogInformation("Successfully processed and archived: {FilePath}", filePath);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file: {FilePath}", filePath);
                FileHelper.MoveToError(filePath, ex);
            }
        }
    }


}
