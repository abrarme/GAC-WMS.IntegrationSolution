using GAC_WMS.IntegrationSolution.Helper;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Services.Implementation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Text.Json;

namespace GAC_WMS.IntegrationSolution.Jobs
{
    public class FilePollingJob : IJob
    {
        private readonly ILogger<FilePollingJob> _logger;
        private readonly IFileProcessorFactory _factory;
        private readonly IConfiguration _config;
        private readonly IFileErrorHandler _fileErrorHandler;

        public FilePollingJob(ILogger<FilePollingJob> logger, IFileProcessorFactory factory, IConfiguration config,IFileErrorHandler fileErrorHandler)
        {
            _logger = logger;
            _factory = factory;
            _config = config;
            _fileErrorHandler = fileErrorHandler;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {

                var directoryConfigs = _config.GetSection("FilePolling:Directories").Get<List<DirectoryConfig>>();


                foreach (var dirConfig in directoryConfigs)
                {
                    
                    if (string.IsNullOrEmpty(dirConfig.Path) || !Directory.Exists(dirConfig.Path))
                    {
                        _logger.LogWarning("Inbound directory not found or not configured: {Path}", dirConfig.Path);
                        return;
                    }
                    var files = Directory.GetFiles(dirConfig.Path);
                    if (!files.Any())
                    {
                        _logger.LogInformation("No files to process in {Path}", dirConfig.Path);
                        return;
                    }
                    foreach (var file in files)
                    {
                        try
                        {
                            var processor = _factory.GetProcessor(Path.GetExtension(file));
                            await processor.ProcessAsync(file,dirConfig.Endpoint);
                        }
                        catch (Exception ex)
                        {
                         
                            _fileErrorHandler.MoveToError(file, ex);
                            _logger.LogError(ex, "Failed to process file: {File}", file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file polling");
            }

        }
    }

}
