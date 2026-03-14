// Controllers/TaskControllerTests.cs
using Application.Dto.Tasks;
using Application.Dto.Users;
using AutoMapper;
using Core.Entities;
using DotnetBackend.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Infra.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Tests.Controllers;

public class TaskControllerTests
{
    #region Test Setup

    private readonly Mock<IValidator<CreateTaskInput>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateTaskInput>> _mockUpdateValidator;
    private readonly Mock<ILogger<TaskController>> _mockLogger;
    private readonly Mock<ITaskItemRepository> _mockTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _mockCreateValidator = new Mock<IValidator<CreateTaskInput>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateTaskInput>>();
        _mockLogger = new Mock<ILogger<TaskController>>();
        _mockTaskRepository = new Mock<ITaskItemRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();

        _controller = new TaskController(
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object,
            _mockLogger.Object,
            _mockTaskRepository.Object,
            _mockUserRepository.Object,
            _mockMapper.Object
        );
    }

    #endregion

    #region GetTasks Tests

    [Fact]
    public async Task GetTasks_WithoutFilter_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            CreateTaskItem(1, "Task 1", "pending", 1),
            CreateTaskItem(2, "Task 2", "completed", 1)
        };

        _mockTaskRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTasks();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value as TasksResponse;
        response.Should().NotBeNull();
        response.Tasks.Count().Should().Be(2);
        response.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetTasks_WithStatusFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            CreateTaskItem(1, "Task 1", "pending", 1),
            CreateTaskItem(2, "Task 2", "completed", 1),
            CreateTaskItem(3, "Task 3", "pending", 2)
        };

        _mockTaskRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTasks(status: "pending");

        // Assert
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as TasksResponse;

        response.Tasks.Count().Should().Be(2);
        response.Tasks.All(t => t.Status.Equals("pending", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task GetTasks_WhenNoTasks_ReturnsEmptyList()
    {
        // Arrange
        _mockTaskRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await _controller.GetTasks();

        // Assert
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as TasksResponse;

        response.Tasks.Should().BeEmpty();
        response.Count.Should().Be(0);
    }

    #endregion

    #region GetTaskById Tests

    [Fact]
    public async Task GetTaskById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var task = CreateTaskItem(taskId, "Test Task", "pending", 1);
        var taskDto = CreateTaskItemDto(taskId, "Test Task", "pending", 1);

        _mockTaskRepository.Setup(repo => repo.GetAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _mockMapper.Setup(m => m.Map<TaskItemDto>(task))
            .Returns(taskDto);

        // Act
        var result = await _controller.GetTaskById(taskId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedTask = okResult.Value as TaskItemDto;
        returnedTask.Should().BeEquivalentTo(taskDto);
    }

    [Fact]
    public async Task GetTaskById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var taskId = 999;
        _mockTaskRepository.Setup(repo => repo.GetAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _controller.GetTaskById(taskId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region CreateTask Tests

    [Fact]
    public async Task CreateTask_WithValidInput_ReturnsCreatedAtAction()
    {
        // Arrange
        var input = CreateValidCreateTaskInput();
        var taskEntity = CreateTaskItem(0, input.Title, "pending", input.UserId);
        var createdTask = CreateTaskItem(1, input.Title, "pending", input.UserId);
        var taskDto = CreateTaskItemDto(1, input.Title, "pending", input.UserId);

        // Mock validation success
        _mockCreateValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Mock user exists check
        _mockUserRepository.Setup(repo => repo.GetAsync(input.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = input.UserId });

        // Mock mapping
        _mockMapper.Setup(m => m.Map<TaskItem>(input))
            .Returns(taskEntity);
        _mockMapper.Setup(m => m.Map<TaskItemDto>(createdTask))
            .Returns(taskDto);

        // Mock repository create
        _mockTaskRepository.Setup(repo => repo.CreateAsync(taskEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _controller.CreateTask(input);

        // Assert
        var createdAtResult = result as CreatedAtActionResult;
        createdAtResult.Should().NotBeNull();
        createdAtResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdAtResult.ActionName.Should().Be(nameof(TaskController.GetTaskById));

        var returnedTask = createdAtResult.Value as TaskItemDto;
        returnedTask.Should().BeEquivalentTo(taskDto);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Task created with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateTask_WhenExceptionThrown_ReturnsProblem()
    {
        // Arrange
        var input = CreateValidCreateTaskInput();

        _mockCreateValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUserRepository.Setup(repo => repo.GetAsync(input.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = input.UserId });

        _mockMapper.Setup(m => m.Map<TaskItem>(input))
            .Throws(new Exception("Database connection failed"));

        // Act
        var result = await _controller.CreateTask(input);

        // Assert
        var problemResult = result as ObjectResult;
        problemResult.Should().NotBeNull();
        problemResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    #endregion

    #region UpdateTask Tests

    [Fact]
    public async Task UpdateTask_WithValidInput_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var existingTask = CreateTaskItem(taskId, "Old Title", "pending", 1);
        var input = new UpdateTaskInput
        {
            Title = "New Title",
            Status = "completed"
        };
        var updatedTask = CreateTaskItem(taskId, "New Title", "completed", 1);
        var taskDto = CreateTaskItemDto(taskId, "New Title", "completed", 1);

        _mockTaskRepository.Setup(repo => repo.GetAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockTaskRepository.Setup(repo => repo.UpdateAsync(existingTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        _mockMapper.Setup(m => m.Map<TaskItemDto>(updatedTask))
            .Returns(taskDto);

        // Act
        var result = await _controller.UpdateTask(taskId, input);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedTask = okResult.Value as TaskItemDto;
        returnedTask.Should().BeEquivalentTo(taskDto);

        // Verify partial update occurred
        existingTask.Title.Should().Be("New Title");
        existingTask.Status.Should().Be("completed");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Task updated with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateTask_WithNonExistentTask_ReturnsNotFound()
    {
        // Arrange
        var taskId = 999;
        var input = new UpdateTaskInput { Title = "New Title" };

        _mockTaskRepository.Setup(repo => repo.GetAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _controller.UpdateTask(taskId, input);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { error = $"Task with ID {taskId} not found" });
    }

    [Fact]
    public async Task UpdateTask_WithPartialData_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var taskId = 1;
        var originalUserId = 1;
        var existingTask = CreateTaskItem(taskId, "Original Title", "pending", originalUserId);
        var input = new UpdateTaskInput
        {
            Title = "Updated Title"
            // Status and UserId not provided (null)
        };
        var updatedTask = CreateTaskItem(taskId, "Updated Title", "pending", originalUserId);
        var taskDto = CreateTaskItemDto(taskId, "Updated Title", "pending", originalUserId);

        _mockTaskRepository.Setup(repo => repo.GetAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockTaskRepository.Setup(repo => repo.UpdateAsync(existingTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        _mockMapper.Setup(m => m.Map<TaskItemDto>(updatedTask))
            .Returns(taskDto);

        // Act
        var result = await _controller.UpdateTask(taskId, input);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        // Verify only title was updated, status and userId remain unchanged
        existingTask.Title.Should().Be("Updated Title");
        existingTask.Status.Should().Be("pending"); // Unchanged
        existingTask.UserId.Should().Be(originalUserId); // Unchanged
    }

    [Fact]
    public async Task UpdateTask_WhenExceptionThrown_ReturnsProblem()
    {
        // Arrange
        var taskId = 1;
        var existingTask = CreateTaskItem(taskId, "Title", "pending", 1);
        var input = new UpdateTaskInput { Title = "New Title" };

        _mockTaskRepository.Setup(repo => repo.GetAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockTaskRepository.Setup(repo => repo.UpdateAsync(existingTask, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Update failed"));

        // Act
        var result = await _controller.UpdateTask(taskId, input);

        // Assert
        var problemResult = result as ObjectResult;
        problemResult.Should().NotBeNull();
        problemResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    #endregion

    #region Helper Methods

    private static TaskItem CreateTaskItem(int id, string title, string status, int userId)
    {
        return new TaskItem
        {
            Id = id,
            Title = title,
            Status = status,
            UserId = userId,
            CreationTime = DateTime.UtcNow
        };
    }

    private static TaskItemDto CreateTaskItemDto(int id, string title, string status, int userId)
    {
        return new TaskItemDto
        {
            Title = title,
            Status = status,
            UserId = userId
        };
    }

    private static CreateTaskInput CreateValidCreateTaskInput()
    {
        return new CreateTaskInput
        {
            Title = "New Task",
            UserId = 1
        };
    }

    #endregion
}