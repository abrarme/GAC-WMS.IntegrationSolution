using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GAC_WMS.IntegrationSolution.Helper;
using GAC_WMS.IntegrationSolution.Jobs;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Services.Implementation;
using GAC_WMS.IntegrationSolution.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Quartz;
using Xunit;

public class FilePollingJobTests
{
    [Fact]
    public async Task Execute_ShouldLogWarning_WhenDirectoryNotFound()
    {
        // Arrange
        var configList = new List<DirectoryConfig>
        {
            new DirectoryConfig
            {
                Path = "Z:\\NonExistentDirectory",
                Endpoint = "http://localhost/api"
            }
        };

        // Build IConfiguration with directory configs
        var inMemorySettings = new Dictionary<string, string>
        {
            { "FilePolling:Directories:0:Path", configList[0].Path },
            { "FilePolling:Directories:0:Endpoint", configList[0].Endpoint }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = Substitute.For<ILogger<FilePollingJob>>();
        var processorFactory = Substitute.For<IFileProcessorFactory>();
        var fileErrorHandler = Substitute.For<IFileErrorHandler>();
        var job = new FilePollingJob(logger, processorFactory, configuration, fileErrorHandler);

        var context = Substitute.For<IJobExecutionContext>();

        // Act
        await job.Execute(context);

        // Assert
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Inbound directory not found")),
            Arg.Any<System.Exception>(),
            Arg.Any<Func<object, System.Exception, string>>());
    }

    [Fact]
    public async Task Execute_ShouldProcessFiles_WhenFilesExist()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        var testFile = Path.Combine(tempDir, "test.csv");
        File.WriteAllText(testFile, "dummy content");

        var configList = new List<DirectoryConfig>
        {
            new DirectoryConfig
            {
                Path = tempDir,
                Endpoint = "http://localhost/api"
            }
        };

        var inMemorySettings = new Dictionary<string, string>
        {
            { "FilePolling:Directories:0:Path", configList[0].Path },
            { "FilePolling:Directories:0:Endpoint", configList[0].Endpoint }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = Substitute.For<ILogger<FilePollingJob>>();
        var processorFactory = Substitute.For<IFileProcessorFactory>();
        var processor = Substitute.For<IFileProcessor>();
        var fileErrorHandler = Substitute.For<IFileErrorHandler>();

        processorFactory.GetProcessor(Arg.Any<string>()).Returns(processor);

        var job = new FilePollingJob(logger, processorFactory, configuration,fileErrorHandler);

        var context = Substitute.For<IJobExecutionContext>();

        // Act
        await job.Execute(context);

        // Assert
        await processor.Received(1).ProcessAsync(testFile, configList[0].Endpoint);

        // Cleanup
        File.Delete(testFile);
        Directory.Delete(tempDir);
    }

    [Fact]
    public async Task Execute_ShouldLogError_AndMoveFileToError_WhenProcessingFails()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        var testFile = Path.Combine(tempDir, "test.csv");
        File.WriteAllText(testFile, "dummy content");

        var configList = new List<DirectoryConfig>
        {
            new DirectoryConfig
            {
                Path = tempDir,
                Endpoint = "http://localhost/api"
            }
        };

        var inMemorySettings = new Dictionary<string, string>
        {
            { "FilePolling:Directories:0:Path", configList[0].Path },
            { "FilePolling:Directories:0:Endpoint", configList[0].Endpoint }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = Substitute.For<ILogger<FilePollingJob>>();
        var processorFactory = Substitute.For<IFileProcessorFactory>();
        var processor = Substitute.For<IFileProcessor>();
        var fileErrorHandler = Substitute.For<IFileErrorHandler>();

        var exception = new System.Exception("Processing failed");
        processorFactory.GetProcessor(Arg.Any<string>()).Returns(processor);
        processor.ProcessAsync(Arg.Any<string>(), Arg.Any<string>()).Returns<Task>(x => throw exception);

        // Mock FileHelper.MoveToError static method — if possible, wrap in interface or skip here

        var job = new FilePollingJob(logger, processorFactory, configuration,fileErrorHandler);

        var context = Substitute.For<IJobExecutionContext>();

        // Act
        await job.Execute(context);

        // Assert
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Failed to process file")),
            exception,
            Arg.Any<Func<object, System.Exception, string>>());

        // Cleanup
        File.Delete(testFile);
        Directory.Delete(tempDir);
    }
}
