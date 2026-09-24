using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Features.Bookings.CreateBooking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly CreateBookingService _createBookingService;

    public BookingsController(CreateBookingService createBookingService)
    {
        _createBookingService = createBookingService;
    }

    [Authorize(Policy = AuthorizationPolicies.CreateBooking)]
    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _createBookingService.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Room not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "Room is not available." ||
                result.Error == "Room is not available for the selected dates.")
            {
                return Conflict(new
                {
                    message = result.Error
                });
            }

            return BadRequest(new
            {
                message = result.Error
            });
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}