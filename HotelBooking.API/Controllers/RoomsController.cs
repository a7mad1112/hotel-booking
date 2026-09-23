using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Features.Rooms.CreateRoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly CreateRoomService _createRoomService;

    public RoomsController(CreateRoomService createRoomService)
    {
        _createRoomService = createRoomService;
    }

    [Authorize(Policy = AuthorizationPolicies.ManageRooms)]
    [HttpPost]
    public async Task<ActionResult<CreateRoomResponse>> Create(CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");

        var result = await _createRoomService.CreateAsync(request, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Hotel not found." || result.Error == "Room type not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to create rooms for this hotel.")
            {
                return Forbid();
            }

            return Conflict(new
            {
                message = result.Error
            });
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}