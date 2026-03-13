using System.Diagnostics.Metrics;

public class TaskApiMetrics
{
    private readonly Counter<long> _taskRequestCounter;
    private readonly Counter<long> _taskErrorCounter;
    private readonly Histogram<double> _taskRequestDuration;

    public TaskApiMetrics()
    {
        // Create Meter inside the constructor
        var meter = new Meter("TaskManagement.API", "1.0.0");

        _taskRequestCounter = meter.CreateCounter<long>(
            "task.api.requests.total",
            description: "Total number of task API requests");

        _taskErrorCounter = meter.CreateCounter<long>(
            "task.api.errors.total",
            description: "Total number of task API errors");

        _taskRequestDuration = meter.CreateHistogram<double>(
            "task.api.request.duration",
            unit: "ms",
            description: "Duration of task API requests");
    }

    // Rest of the methods remain the same
    public void RecordRequest(string endpoint, string method, string status)
    {
        _taskRequestCounter.Add(1,
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("status", status));
    }

    public void RecordError(string endpoint, string errorType)
    {
        _taskErrorCounter.Add(1,
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("error_type", errorType));
    }

    public void RecordRequestDuration(double durationMs, string endpoint, string method)
    {
        _taskRequestDuration.Record(durationMs,
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("method", method));
    }
}