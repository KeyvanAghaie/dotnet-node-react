// Controllers/UserControllerTests.cs
using Application.Dto.Users;
using AutoMapper;
using Core.Entities;
using DotnetBackend.Data;
using DotnetBackend.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Infra.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTests.Tests.Helpers;

namespace UnitTests.Tests.Controllers;

public class UserControllerTests
{
    #region Test Setup

    private readonly Mock<IValidator<CreateUserInput>> _mockValidator;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly Mock<DataStore> _mockDataStore;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        // Initialize mocks
        _mockValidator = new Mock<IValidator<CreateUserInput>>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _mockDataStore = new Mock<DataStore>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();

        // Create controller instance with mocks
        _controller = new UserController(
            _mockValidator.Object,
            _mockLogger.Object,
            _mockDataStore.Object,
            _mockUserRepository.Object,
            _mockMapper.Object
        );
    }

    #endregion

    #region GetUserById Tests

    [Fact]
    public async Task GetUserById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUserEntity();
        var userDto = TestDataFactory.CreateUserDto();

        _mockUserRepository.Setup(repo => repo.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedUser = okResult.Value as UserDto;
        returnedUser.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(repo => repo.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region GetAllUsers Tests

    [Fact]
    public async Task Users_GetAll_ReturnsOkWithUsersResponse()
    {
        // Arrange
        var users = new List<User>
        {
            TestDataFactory.CreateUserEntity(),
            new User { Id = 2, Email = "test2@example.com", Name = "Jane", Role = "admin" }
        };

        _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _controller.Users();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value as UsersResponse;
        response.Should().NotBeNull();
        response.Users.Count().Should().Be(2);
        response.Count.Should().Be(2);
    }

    [Fact]
    public async Task Users_GetAll_WhenNoUsers_ReturnsEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.Users();

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult.Value as UsersResponse;
        response.Users.Should().BeEmpty();
        response.Count.Should().Be(0);
    }

    #endregion

    #region CreateUser Tests

    [Fact]
    public async Task Users_Post_WithValidInput_ReturnsCreatedAtAction()
    {
        // Arrange
        var input = TestDataFactory.CreateValidUserInput();
        var userEntity = TestDataFactory.CreateUserEntity();
        var createdUser = TestDataFactory.CreateUserEntity();
        var userDto = TestDataFactory.CreateUserDto();

        // Mock validation
        var validationResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Mock duplicate email check
        _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Mock mapping
        _mockMapper.Setup(m => m.Map<User>(input))
            .Returns(userEntity);
        _mockMapper.Setup(m => m.Map<UserDto>(createdUser))
            .Returns(userDto);

        // Mock repository create
        _mockUserRepository.Setup(repo => repo.CreateAsync(userEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.Users(input);

        // Assert
        var createdAtResult = result as CreatedAtActionResult;
        createdAtResult.Should().NotBeNull();
        createdAtResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdAtResult.ActionName.Should().Be(nameof(UserController.Users));

        var returnedUser = createdAtResult.Value as UserDto;
        returnedUser.Should().BeEquivalentTo(userDto);

        // Verify logger was called
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User created with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Users_Post_WhenExceptionThrown_ReturnsProblem()
    {
        // Arrange
        var input = TestDataFactory.CreateValidUserInput();

        // Mock validation success
        _mockValidator.Setup(v => v.ValidateAsync(input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Mock duplicate email check
        _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Mock mapping
        _mockMapper.Setup(m => m.Map<User>(input))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _controller.Users(input);

        // Assert
        var problemResult = result as ObjectResult;
        problemResult.Should().NotBeNull();
        problemResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        // Verify error was logged
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
}