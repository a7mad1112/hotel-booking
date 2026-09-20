using FluentValidation;
using HotelBooking.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterService _registerService;
    private readonly IValidator<RegisterRequest> _validator;

    public AuthController(
        RegisterService registerService,
        IValidator<RegisterRequest> validator)
    {
        _registerService = registerService;
        _validator = validator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _validator.ValidateAsync(request, cancellationToken);

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
}