using Application.Dto.Users;
using Application.Services;
using AutoMapper;
using Core.Entities;
using FluentValidation;
using Infra.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IValidator<CreateUserInput> _registerValidator;
    private readonly ILogger<AuthController> _logger;
    private readonly IMapper _mapper;


    public AuthController(
        IUserRepository userRepository,
        IJwtService jwtService,
        IValidator<CreateUserInput> registerValidator,
        ILogger<AuthController> logger,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _registerValidator = registerValidator;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Register a new user and return JWT token
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] CreateUserInput input,
        CancellationToken cancellationToken = default)
    {
        // Validate input
        var validationResult = await _registerValidator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }

        // Check duplicate email
        var existingUsers = await _userRepository.GetAllAsync(cancellationToken);
        if (existingUsers.Any(u => u.Email.Equals(input.Email, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("Email", "Email already exists");
            return ValidationProblem(ModelState);
        }

        // Create user
        var user = _mapper.Map<User>(input);

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);
        var token = _jwtService.GenerateToken(createdUser);

        _logger.LogInformation("User registered: {UserId}", createdUser.Id);

        return CreatedAtAction(
            nameof(Login),
            new AuthResponse
            {
                Token = token,
                User = _mapper.Map<UserDto>(user),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
    }

    /// <summary>
    /// Login with email and get JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginInput input,
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u =>
            u.Email.Equals(input.Email, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });
    }
}

public record LoginInput(string Email);
public record AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}