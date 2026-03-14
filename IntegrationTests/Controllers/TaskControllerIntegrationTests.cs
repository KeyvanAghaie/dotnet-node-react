using Application.Dto.Tasks;
using Application.Dto.Users;
using DotnetBackend.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Controllers;

public class TaskControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TaskControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region GetTasks Tests

    [Fact]
    public async Task GetTasks_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTasks_ReturnsTasksList()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasksResponse = await response.Content.ReadFromJsonAsync<TasksResponse>();
        tasksResponse.Should().NotBeNull();
        tasksResponse.Tasks.Should().NotBeNull();
        tasksResponse.Count.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetTasks_WithNonExistentStatusFilter_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks?status=nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasksResponse = await response.Content.ReadFromJsonAsync<TasksResponse>();
        tasksResponse.Tasks.Should().BeEmpty();
        tasksResponse.Count.Should().Be(0);
    }

    #endregion

    #region GetTaskById Tests

    [Fact]
    public async Task GetTaskById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region CreateTask Tests

    [Fact]
    public async Task CreateTask_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidTask = new CreateTaskInput
        {
            Title = "", // Invalid: empty title
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", invalidTask);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_WithMissingTitle_ReturnsValidationError()
    {
        // Arrange
        var newTask = new CreateTaskInput
        {
            // Title is missing
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion


    #region Helper Methods

    private async Task<UserDto> CreateUserAsync(string email)
    {
        var newUser = new CreateUserInput
        {
            Email = email,
            Name = "Test User",
            Role = "user"
        };

        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    #endregion
}