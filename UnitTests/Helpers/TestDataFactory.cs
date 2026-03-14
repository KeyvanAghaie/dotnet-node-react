using Application.Dto.Tasks;
using Application.Dto.Users;
using Core.Entities;
using Core.Entities;

namespace UnitTests.Tests.Helpers;

public static class TestDataFactory
{
    public static CreateUserInput CreateValidUserInput()    
    {
        return new CreateUserInput
        {
            Email = "test@example.com",
            Name = "John",
            Role = "admin"
        };
    }

    public static CreateUserInput CreateInvalidUserInput()
    {
        return new CreateUserInput
        {
            Email = "", // Invalid email
            Name = "John",
            Role = "admin"
        };
    }

    public static User CreateUserEntity()
    {
        return new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "John",
            Role = "admin",
            CreationTime = DateTime.UtcNow
        };
    }

    public static UserDto CreateUserDto()
    {
        return new UserDto
        {
            Id = 1,
            Email = "test@example.com",
            Name = "John",
            Role = "admin"
        };
    }



    public static CreateTaskInput CreateValidCreateTaskInput()
    {
        return new CreateTaskInput
        {
            Title = "Test Task",
            UserId = 1
        };
    }

    public static TaskItem CreateTaskItem(int id = 1, string title = "Test Task")
    {
        return new TaskItem
        {
            Id = id,
            Title = title,
            Status = "pending",
            UserId = 1,
            CreationTime = DateTime.UtcNow
        };
    }

    public static TaskItemDto CreateTaskItemDto(int id = 1, string title = "Test Task")
    {
        return new TaskItemDto
        {
            Title = title,
            Status = "pending",
            UserId = 1
        };
    }
}