using System.Text.Json.Serialization;
using Core.Entities;

namespace DotnetBackend.Models;

public class UsersResponse
{
    [JsonPropertyName("users")]
    public List<User> Users { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class TasksResponse
{
    [JsonPropertyName("tasks")]
    public List<TaskItem> Tasks { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class UsersStats
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class TasksStats
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("pending")]
    public int Pending { get; set; }

    [JsonPropertyName("inProgress")]
    public int InProgress { get; set; }

    [JsonPropertyName("completed")]
    public int Completed { get; set; }
}

public class StatsResponse
{
    [JsonPropertyName("users")]
    public UsersStats Users { get; set; } = new();

    [JsonPropertyName("tasks")]
    public TasksStats Tasks { get; set; } = new();
}

public class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}




