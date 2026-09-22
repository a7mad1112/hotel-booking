using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Hotels.CreateHotel;
using HotelBooking.Application.Features.Hotels.DeleteHotel;
using HotelBooking.Application.Features.Hotels.GetHotelById;
using HotelBooking.Application.Features.Hotels.GetHotels;
using HotelBooking.Application.Features.Hotels.UpdateHotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly CreateHotelService _createHotelService;
    private readonly GetHotelsService _getHotelsService;
    private readonly GetHotelByIdService _getHotelByIdService;
    private readonly DeleteHotelService _deleteHotelService;
    private readonly UpdateHotelService _updateHotelService;

    public HotelsController(
        CreateHotelService createHotelService,
        GetHotelsService getHotelsService,
        GetHotelByIdService getHotelByIdService,
        DeleteHotelService deleteHotelService,
        UpdateHotelService updateHotelService)
    {
        _createHotelService = createHotelService;
        _getHotelsService = getHotelsService;
        _getHotelByIdService = getHotelByIdService;
        _deleteHotelService = deleteHotelService;
        _updateHotelService = updateHotelService;
    }


    [Authorize(Policy = AuthorizationPolicies.ManageHotels)]
    [HttpPost]
    public async Task<ActionResult<CreateHotelResponse>> Create(
        CreateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _createHotelService.CreateAsync(
                request,
                cancellationToken);


        if (!result.IsSuccess)
        {
            if (result.Error == "City not found."
                || result.Error == "Owner not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }


            return Conflict(new
            {
                message = result.Error
            });
        }


        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Value!.Id
            },
            result.Value);
    }


    [HttpGet]
    public async Task<ActionResult<PagedResult<GetHotelsResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var hotels =
            await _getHotelsService.GetAllAsync(
                request,
                cancellationToken);


        return Ok(hotels);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetHotelByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _getHotelByIdService.GetAsync(
                id,
                cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound(new
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


        var result = await _deleteHotelService.DeleteAsync(
            id,
            currentUserId,
            isAdmin,
            cancellationToken);


        if (!result.IsSuccess)
        {
            if (result.Error == "Hotel not found.")
            {
                return NotFound(new { message = result.Error });
            }

            if (result.Error ==
                "You are not allowed to delete this hotel.")
            {
                return Forbid();
            }

            return BadRequest(new
            {
                message = result.Error
            });
        }

        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateHotelResponse>> Update(
        int id,
        UpdateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(
                userIdClaim,
                out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin =
            User.IsInRole("Admin");

        var result =
            await _updateHotelService.UpdateAsync(
                id,
                request,
                currentUserId,
                isAdmin,
                cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Hotel not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error ==
                "You are not allowed to update this hotel.")
            {
                return Forbid();
            }

            if (result.Error == "City not found.")
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
}