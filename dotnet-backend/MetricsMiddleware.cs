using System.Diagnostics;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TaskApiMetrics _metrics;
    private readonly ILogger<MetricsMiddleware> _logger;

    public MetricsMiddleware(RequestDelegate next, TaskApiMetrics metrics, ILogger<MetricsMiddleware> logger)
    {
        _next = next;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var endpoint = context.Request.Path;
        var method = context.Request.Method;

        try
        {
            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode.ToString();

            _metrics.RecordRequestDuration(stopwatch.ElapsedMilliseconds, endpoint, method);

            if (statusCode.StartsWith("2") || statusCode.StartsWith("3"))
            {
                _metrics.RecordRequest(endpoint, method, statusCode);
            }
            else if (statusCode.StartsWith("4") || statusCode.StartsWith("5"))
            {
                _metrics.RecordError(endpoint, $"HTTP{statusCode}");
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metrics.RecordRequestDuration(stopwatch.ElapsedMilliseconds, endpoint, method);
            _metrics.RecordError(endpoint, ex.GetType().Name);
            throw;
        }
    }
}
