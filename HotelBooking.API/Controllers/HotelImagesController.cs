using HotelBooking.API.Extensions;
using HotelBooking.API.Features.Hotels.UploadHotelImage;
using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Features.Hotels.DeleteHotelImage;
using HotelBooking.Application.Features.Hotels.UploadHotelImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:int}/images")]
public class HotelImagesController : ControllerBase
{
    private readonly UploadHotelImageService _uploadHotelImageService;
    private readonly DeleteHotelImageService _deleteHotelImageService;

    public HotelImagesController(
        UploadHotelImageService uploadHotelImageService,
        DeleteHotelImageService deleteHotelImageService)
    {
        _uploadHotelImageService = uploadHotelImageService;
        _deleteHotelImageService = deleteHotelImageService;
    }

    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadHotelImageResponse>> UploadImage(
        int hotelId,
        [FromForm] UploadHotelImageRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var image = request.Image!;
        await using var stream = image.OpenReadStream();

        var imageUpload = new ImageUpload
        {
            Content = stream,
            FileName = image.FileName,
            ContentType = image.ContentType
        };

        var result = await _uploadHotelImageService.UploadAsync(
            hotelId,
            imageUpload,
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
    public async Task<IActionResult> DeleteImage(int hotelId, int imageId, CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _deleteHotelImageService.DeleteAsync(
            hotelId,
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
