using System.Security.Claims;
using FluentValidation;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.API.Extensions;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterService _registerService;
    private readonly LoginService _loginService;

    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        RegisterService registerService,
        LoginService loginService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _registerService = registerService;
        _loginService = loginService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _registerValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return this.ValidationProblem(validationResult);
        }

        var result = await _registerService.RegisterAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Conflict(new
            {
                statusCode = StatusCodes.Status409Conflict,
                message = result.Error
            });
        }

        var user = result.Value!;

        return StatusCode(
            StatusCodes.Status201Created,
            new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email
            });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _loginValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return this.ValidationProblem(validationResult);
        }

        var result = await _loginService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new
            {
                statusCode = StatusCodes.Status401Unauthorized,
                message = result.Error
            });
        }

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