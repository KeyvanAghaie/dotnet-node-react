using System.Diagnostics;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        // Capture request details BEFORE processing
        var method = request.Method;
        var path = request.Path;
        var queryString = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;

        _logger.LogInformation(
            "Incoming Request: {HttpMethod} {Path}{QueryString} started at {Timestamp:yyyy-MM-dd HH:mm:ss.fff}",
            method,
            path,
            queryString,
            DateTime.UtcNow);

        try
        {
            // Process the request
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Capture response details AFTER processing
            var statusCode = context.Response.StatusCode;
            var durationMs = stopwatch.ElapsedMilliseconds;

            var logLevel = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

            _logger.Log(logLevel,
                "Request completed: {HttpMethod} {Path}{QueryString} - Status {StatusCode} - Duration {DurationMs}ms",
                method,
                path,
                queryString,
                statusCode,
                durationMs);
        }
    }
}