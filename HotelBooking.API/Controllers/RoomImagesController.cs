using HotelBooking.API.Extensions;
using HotelBooking.API.Features.Rooms.UploadRoomImage;
using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Features.Rooms.DeleteRoomImage;
using HotelBooking.Application.Features.Rooms.UploadRoomImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/rooms/{roomId:int}/images")]
public class RoomImagesController : ControllerBase
{
    private readonly UploadRoomImageService _uploadRoomImageService;
    private readonly DeleteRoomImageService _deleteRoomImageService;

    public RoomImagesController(
        UploadRoomImageService uploadRoomImageService,
        DeleteRoomImageService deleteRoomImageService)
    {
        _uploadRoomImageService = uploadRoomImageService;
        _deleteRoomImageService = deleteRoomImageService;
    }

    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadRoomImageResponse>> UploadImage(
        int roomId,
        [FromForm] UploadRoomImageRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
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
            User.IsAdmin(),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int roomId, int imageId, CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _deleteRoomImageService.DeleteAsync(
            roomId,
            imageId,
            currentUserId,
            User.IsAdmin(),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return NoContent();
    }
}
