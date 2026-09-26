using HotelBooking.API.Authorization;
using HotelBooking.API.Extensions;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms.GetRooms;
using HotelBooking.Application.Features.RoomTypes.CreateRoomType;
using HotelBooking.Application.Features.RoomTypes.DeleteRoomType;
using HotelBooking.Application.Features.RoomTypes.GetRoomTypeById;
using HotelBooking.Application.Features.RoomTypes.GetRoomTypes;
using HotelBooking.Application.Features.RoomTypes.UpdateRoomType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/room-types")]
public class RoomTypesController : ControllerBase
{
    private readonly CreateRoomTypeService _createRoomTypeService;
    private readonly GetRoomTypesService _getRoomTypesService;
    private readonly GetRoomTypeByIdService _getRoomTypeByIdService;
    private readonly UpdateRoomTypeService _updateRoomTypeService;
    private readonly DeleteRoomTypeService _deleteRoomTypeService;

    public RoomTypesController(
        CreateRoomTypeService createRoomTypeService,
        GetRoomTypesService getRoomTypesService,
        GetRoomTypeByIdService getRoomTypeByIdService,
        UpdateRoomTypeService updateRoomTypeService,
        DeleteRoomTypeService deleteRoomTypeService)
    {
        _createRoomTypeService = createRoomTypeService;
        _getRoomTypesService = getRoomTypesService;
        _getRoomTypeByIdService = getRoomTypeByIdService;
        _updateRoomTypeService = updateRoomTypeService;
        _deleteRoomTypeService = deleteRoomTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetRoomTypesResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _getRoomTypesService.GetAllAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetRoomTypeByIdResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getRoomTypeByIdService.GetAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageRoomTypes)]
    [HttpPost]
    public async Task<ActionResult<CreateRoomTypeResponse>> Create(
        CreateRoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createRoomTypeService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Value!.Id
            },
            result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageRoomTypes)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateRoomTypeResponse>> Update(
        int id,
        UpdateRoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateRoomTypeService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.ManageRoomTypes)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _deleteRoomTypeService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return NoContent();
    }
}