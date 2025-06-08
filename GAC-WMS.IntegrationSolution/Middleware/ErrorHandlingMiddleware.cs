using System.Net;
using System.Text.Json;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = Guid.NewGuid().ToString();
            _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", traceId);
            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        var code = HttpStatusCode.InternalServerError;

        var errorDetails = new
        {
            status = (int)code,
            message = "An unexpected error occurred.",
            traceId = traceId
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var json = JsonSerializer.Serialize(errorDetails);
        return context.Response.WriteAsync(json);
    }
}
