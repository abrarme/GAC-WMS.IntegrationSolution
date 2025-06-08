using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

public class RetryPolicyHandler : DelegatingHandler
{
    private readonly ILogger<RetryPolicyHandler> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public RetryPolicyHandler(ILogger<RetryPolicyHandler> logger)
    {
        _logger = logger;

        _retryPolicy = Policy<HttpResponseMessage>
            .HandleResult(r => IsTransientFailure(r))
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryAttempt} due to {StatusCode}. Waiting {Delay}s before next retry.",
                        retryAttempt,
                        outcome.Result?.StatusCode,
                        timespan.TotalSeconds);
                });
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _retryPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }

    private bool IsTransientFailure(HttpResponseMessage response)
    {
        if (response == null)
            return true;

        var statusCode = (int)response.StatusCode;

        // Retry on:
        //  - 5xx server errors (500-599)
        //  - 408 Request Timeout
        // Do NOT retry on 400 Bad Request or other 4xx client errors

        return (statusCode >= 500 && statusCode < 600) || response.StatusCode == HttpStatusCode.RequestTimeout;
    }
}
