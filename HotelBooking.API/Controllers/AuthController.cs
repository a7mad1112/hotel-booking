using FluentValidation;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterService _registerService;
    private readonly IValidator<RegisterRequest> _registerValidator;

    private readonly LoginService _loginService;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        RegisterService registerService,
        IValidator<RegisterRequest> registerValidator,
        LoginService loginService,
        IValidator<LoginRequest> loginValidator)
    {
        _registerService = registerService;
        _registerValidator = registerValidator;
        _loginService = loginService;
        _loginValidator = loginValidator;
    }

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
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(
                    error.PropertyName,
                    error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
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
}