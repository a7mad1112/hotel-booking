using HotelBooking.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterService _registerService;

    public AuthController(RegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
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