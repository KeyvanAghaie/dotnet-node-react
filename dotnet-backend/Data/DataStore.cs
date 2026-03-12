using System.Collections.Concurrent;
using Core.Entities;
using DotnetBackend.Models;

namespace DotnetBackend.Data;

public class DataStore
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly List<User> _users;
    private readonly List<TaskItem> _tasks;

    public DataStore()
    {
        _users = new List<User>
        {
            new() { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "developer" },
            new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Role = "designer" },
            new() { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Role = "manager" }
        };

        _tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Implement authentication", Status = "pending", UserId = 1 },
            new() { Id = 2, Title = "Design user interface", Status = "in-progress", UserId = 2 },
            new() { Id = 3, Title = "Review code changes", Status = "completed", UserId = 3 }
        };
    }

    public List<User> GetUsers()
    {
        _lock.EnterReadLock();
        try
        {
            return _users.ToList();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public User? GetUserById(int id)
    {
        _lock.EnterReadLock();
        try
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public List<TaskItem> GetTasks(string? status, string? userId)
    {
        _lock.EnterReadLock();
        try
        {
            IEnumerable<TaskItem> query = _tasks;

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out var uid))
            {
                query = query.Where(t => t.UserId == uid);
            }

            return query.ToList();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public StatsResponse GetStats()
    {
        _lock.EnterReadLock();
        try
        {
            var stats = new StatsResponse
            {
                Users = { Total = _users.Count },
                Tasks = { Total = _tasks.Count }
            };

            foreach (var task in _tasks)
            {
                switch (task.Status)
                {
                    case "pending":
                        stats.Tasks.Pending++;
                        break;
                    case "in-progress":
                        stats.Tasks.InProgress++;
                        break;
                    case "completed":
                        stats.Tasks.Completed++;
                        break;
                }
            }

            return stats;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }



    /// <summary>
    /// Generates a unique ID by incrementing from the maximum existing ID
    /// </summary>
    public int GetNextId()
    {
        _lock.EnterReadLock();
        try
        {
            if (!_users.Any())
            {
                return 1;
            }

            // Find the maximum ID and add 1
            return _users.Max(u => u.Id) + 1;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

}
