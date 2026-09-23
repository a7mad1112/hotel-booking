using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms.CreateRoom;
using HotelBooking.Application.Features.Rooms.DeleteRoom;
using HotelBooking.Application.Features.Rooms.GetRooms;
using HotelBooking.Application.Features.Rooms.UpdateRoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly CreateRoomService _createRoomService;
    private readonly GetRoomsService _getRoomsService;
    private readonly DeleteRoomService _deleteRoomService;
    private readonly UpdateRoomService _updateRoomService;

    public RoomsController(CreateRoomService createRoomService, GetRoomsService getRoomsService,
        DeleteRoomService deleteRoomService, UpdateRoomService updateRoomService)
    {
        _createRoomService = createRoomService;
        _getRoomsService = getRoomsService;
        _deleteRoomService = deleteRoomService;
        _updateRoomService = updateRoomService;
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateRoomResponse>> Update(int id, UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");

        var result = await _updateRoomService.UpdateAsync(id, request, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Room not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to update this room.")
            {
                return Forbid();
            }

            if (result.Error == "Room type not found.")
            {
                return BadRequest(new
                {
                    message = result.Error
                });
            }

            return Conflict(new
            {
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");

        var result = await _deleteRoomService.DeleteAsync(id, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Room not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to delete this room.")
            {
                return Forbid();
            }

            if (result.Error == "Cannot delete a room that has related bookings.")
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

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetRoomsResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var rooms = await _getRoomsService.GetAllAsync(request, cancellationToken);
        return Ok(rooms);
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