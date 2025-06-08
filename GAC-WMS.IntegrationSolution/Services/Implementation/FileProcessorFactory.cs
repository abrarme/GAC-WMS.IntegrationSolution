using GAC_WMS.IntegrationSolution.Services.Interface;

namespace GAC_WMS.IntegrationSolution.Services.Implementation
{
    public interface IFileProcessorFactory
    {
        IFileProcessor GetProcessor(string fileExtension);
    }

    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileProcessorFactory> _logger;

        public FileProcessorFactory(IServiceProvider serviceProvider, ILogger<FileProcessorFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IFileProcessor GetProcessor(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".csv" => _serviceProvider.GetRequiredService<CsvFileProcessor>(),
                ".json" => _serviceProvider.GetRequiredService<JsonFileProcessor>(),
                ".xml" => _serviceProvider.GetRequiredService<XmlFileProcessor>(),
                _ => throw new NotSupportedException($"File extension {fileExtension} is not supported.")
            };
        }
    }


}
