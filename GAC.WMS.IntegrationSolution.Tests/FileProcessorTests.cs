using System.IO;
using System.Threading.Tasks;
using GAC_WMS.IntegrationSolution.Clients;
using GAC_WMS.IntegrationSolution.Services.Implementation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using System.Collections.Generic;
using System.Dynamic;
using GAC.WMS.IntegrationSolution.Tests;
using GAC_WMS.IntegrationSolution.Services.Interface;
using System.Text;

public class FileProcessorTests
{

    [Fact]
    public async Task ProcessAsync_ShouldParseXml_AndPushToClient()
    {
        // Arrange
        var mockClient = Substitute.For<IWmsClient>();
        var mockLogger = Substitute.For<ILogger<XmlFileProcessor>>();
        var mockParser = Substitute.For<IXmlParser>();

        string tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "<root><Customer><Name>John Doe</Name><Email>john@example.com</Email></Customer></root>");

        var dynamicData = new List<dynamic>();
        dynamic obj = new ExpandoObject();
        obj.Name = "John Doe";
        obj.Email = "john@example.com";
        dynamicData.Add(obj);

        mockParser.ParseXmlToDynamicList(Arg.Any<string>(), Arg.Any<string>()).Returns(dynamicData);

        var processor = new XmlFileProcessor(mockClient, mockLogger, mockParser);
        string endPoint = "http://dummy/customer";

        // Act
        await processor.ProcessAsync(tempFile, endPoint);

        // Assert
        IEnumerable<dynamic> capturedList = null;

         mockClient
            .When(x => x.PushDataAsync(Arg.Any<IEnumerable<dynamic>>(), endPoint))
            .Do(callInfo =>
            {
                capturedList = callInfo.Arg<IEnumerable<dynamic>>();
            });

        // invoke the tested method here...

        Assert.NotNull(capturedList);
        var first = capturedList.First() as IDictionary<string, object>;
        Assert.NotNull(first);
        Assert.Equal("John Doe", first["Name"].ToString());
        Assert.Equal("john@example.com", first["Email"].ToString());

        await mockClient.Received(1).PushDataAsync(Arg.Any<IEnumerable<dynamic>>(), endPoint);
        // Clean up temp file
        if (File.Exists(tempFile))
            File.Delete(tempFile);
    }

    //[Fact]
    //public async Task ProcessAsync_ShouldParseXml_AndPushToClient()
    //{
    //    // Arrange
    //    var mockClient = Substitute.For<IWmsClient>();
    //    var mockLogger = Substitute.For<ILogger<XmlFileProcessor>>();
    //    var mockParser = Substitute.For<IXmlParser>();

    //    var processor = new XmlFileProcessor(mockClient, mockLogger, mockParser);
    //    var endPoint = "http://dummy-endpoint/customer";
    //    var xml = @"<Customers>
    //                  <Customer>
    //                      <Name>John Doe</Name>
    //                      <Email>john@example.com</Email>
    //                      <Address>123 Street</Address>
    //                  </Customer>
    //                </Customers>";


    //    var tempFile = Path.GetTempFileName();
    //    await File.WriteAllTextAsync(tempFile, xml);

    //    IEnumerable<dynamic> capturedData = null;

    //    // Capture the pushed data using Arg.Do
    //    mockClient
    //        .When(x => x.PushDataAsync(Arg.Do<IEnumerable<dynamic>>(x => capturedData = x), endPoint))
    //        .Do(_ => { }); // no-op

    //    // Act
    //    await processor.ProcessAsync(tempFile, endPoint);

    //    // Assert
    //    Assert.NotNull(capturedData);

    //    var first = capturedData.First() as IDictionary<string, object>;
    //    Assert.NotNull(first);
    //    Assert.Equal("John Doe", first["Name"]?.ToString());
    //    Assert.Equal("john@example.com", first["Email"]?.ToString());

    //    await mockClient.Received(1).PushDataAsync(Arg.Any<IEnumerable<dynamic>>(), endPoint);

    //    // Clean up
    //    if (File.Exists(tempFile))
    //        File.Delete(tempFile);
    //}


}
