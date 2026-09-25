using System.Security.Claims;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterService _registerService;
    private readonly LoginService _loginService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        RegisterService registerService,
        LoginService loginService,
        ILogger<AuthController> logger)
    {
        _registerService = registerService;
        _loginService = loginService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _registerService.RegisterAsync(request.Email, request.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("User registration request failed.");

            return Conflict(new
            {
                statusCode = StatusCodes.Status409Conflict,
                message = result.Error
            });
        }

        var user = result.Value!;

        _logger.LogInformation("User registration request completed successfully. UserId {UserId}", user.Id);

        return StatusCode(StatusCodes.Status201Created, new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login request failed.");

            return Unauthorized(new
            {
                statusCode = StatusCodes.Status401Unauthorized,
                message = result.Error
            });
        }

        _logger.LogInformation("Login request completed successfully.");

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult GetCurrentUser()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),

            email = User.FindFirstValue(ClaimTypes.Email),

            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}