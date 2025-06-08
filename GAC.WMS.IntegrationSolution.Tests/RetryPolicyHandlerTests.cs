using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GAC.WMS.IntegrationSolution.Tests
{

    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using Microsoft.Extensions.Logging;

    public class RetryPolicyHandlerTests
    {
        [Fact]
        public async Task ShouldRetry_OnTransientFailure()
        {
            // Arrange
            var retryCount = 0;

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    retryCount++;
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                });

            var logger = new LoggerFactory().CreateLogger<RetryPolicyHandler>();
            var retryHandler = new RetryPolicyHandler(logger)
            {
                InnerHandler = mockHandler.Object
            };

            var httpClient = new HttpClient(retryHandler);

            // Act
            var response = await httpClient.GetAsync("http://fake-endpoint");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(4, retryCount); // initial + 3 retries
        }

        [Fact]
        public async Task ShouldNotRetry_OnBadRequest()
        {
            // Arrange
            var retryCount = 0;

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    retryCount++;
                    return new HttpResponseMessage(HttpStatusCode.BadRequest); // 400
                });

            var logger = new LoggerFactory().CreateLogger<RetryPolicyHandler>();
            var retryHandler = new RetryPolicyHandler(logger)
            {
                InnerHandler = mockHandler.Object
            };

            var httpClient = new HttpClient(retryHandler);

            // Act
            var response = await httpClient.GetAsync("http://fake-endpoint");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(1, retryCount); // no retry
        }

        [Fact]
        public async Task ShouldRetry_OnTimeout()
        {
            // Arrange
            var retryCount = 0;

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    retryCount++;
                    return new HttpResponseMessage(HttpStatusCode.RequestTimeout); // 408
                });

            var logger = new LoggerFactory().CreateLogger<RetryPolicyHandler>();
            var retryHandler = new RetryPolicyHandler(logger)
            {
                InnerHandler = mockHandler.Object
            };

            var httpClient = new HttpClient(retryHandler);

            // Act
            var response = await httpClient.GetAsync("http://fake-endpoint");

            // Assert
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
            Assert.Equal(4, retryCount); // 1 attempt + 3 retries
        }
    }



}
