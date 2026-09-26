using HotelBooking.API.Authorization;
using HotelBooking.API.Extensions;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms.CreateRoom;
using HotelBooking.Application.Features.Rooms.DeleteRoom;
using HotelBooking.Application.Features.Rooms.GetRoomById;
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
    private readonly GetRoomByIdService _getRoomByIdService;

    public RoomsController(
        CreateRoomService createRoomService,
        GetRoomsService getRoomsService,
        DeleteRoomService deleteRoomService,
        UpdateRoomService updateRoomService,
        GetRoomByIdService getRoomByIdService)
    {
        _createRoomService = createRoomService;
        _getRoomsService = getRoomsService;
        _deleteRoomService = deleteRoomService;
        _updateRoomService = updateRoomService;
        _getRoomByIdService = getRoomByIdService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetRoomByIdResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getRoomByIdService.GetAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateRoomResponse>> Update(
        int id,
        UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

        var result = await _updateRoomService.UpdateAsync(id, request, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

        var result = await _deleteRoomService.DeleteAsync(id, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetRoomsResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var rooms = await _getRoomsService.GetAllAsync(request, search, cancellationToken);
        return Ok(rooms);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageRooms)]
    [HttpPost]
    public async Task<ActionResult<CreateRoomResponse>> Create(
        CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

        var result = await _createRoomService.CreateAsync(request, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}