using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.API.Features.Rooms.UploadRoomImage;
using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms.CreateRoom;
using HotelBooking.Application.Features.Rooms.DeleteRoom;
using HotelBooking.Application.Features.Rooms.DeleteRoomImage;
using HotelBooking.Application.Features.Rooms.GetRoomById;
using HotelBooking.Application.Features.Rooms.GetRooms;
using HotelBooking.Application.Features.Rooms.UpdateRoom;
using HotelBooking.Application.Features.Rooms.UploadRoomImage;
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

    private readonly UploadRoomImageService _uploadRoomImageService;
    private readonly DeleteRoomImageService _deleteRoomImageService;

    public RoomsController(CreateRoomService createRoomService, GetRoomsService getRoomsService,
        DeleteRoomService deleteRoomService, UpdateRoomService updateRoomService,
        GetRoomByIdService getRoomByIdService, UploadRoomImageService uploadRoomImageService,
        DeleteRoomImageService deleteRoomImageService)
    {
        _createRoomService = createRoomService;
        _getRoomsService = getRoomsService;
        _deleteRoomService = deleteRoomService;
        _updateRoomService = updateRoomService;
        _getRoomByIdService = getRoomByIdService;

        _uploadRoomImageService = uploadRoomImageService;
        _deleteRoomImageService = deleteRoomImageService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetRoomByIdResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getRoomByIdService.GetAsync(id, cancellationToken);

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
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var rooms = await _getRoomsService.GetAllAsync(request, search, cancellationToken);
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

    [Authorize]
    [HttpPost("{roomId:int}/images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadRoomImageResponse>> UploadImage(
        int roomId,
        [FromForm] UploadRoomImageRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        await using var stream = request.Image.OpenReadStream();

        var image = new ImageUpload()
        {
            Content = stream,
            FileName = request.Image.FileName,
            ContentType = request.Image.ContentType
        };

        var result = await _uploadRoomImageService.UploadAsync(
            roomId,
            image,
            currentUserId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Room not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to upload images for this room.")
            {
                return Forbid();
            }

            return BadRequest(new
            {
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{roomId:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int roomId, int imageId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _deleteRoomImageService.DeleteAsync(
            roomId,
            imageId,
            currentUserId,
            User.IsInRole("Admin"),
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Room not found." || result.Error == "Room image not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to delete images for this room.")
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
}