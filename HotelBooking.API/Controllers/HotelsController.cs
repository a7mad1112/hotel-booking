using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.API.Features.Hotels.UploadHotelImage;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Hotels.CreateHotel;
using HotelBooking.Application.Features.Hotels.DeleteHotel;
using HotelBooking.Application.Features.Hotels.GetHotelById;
using HotelBooking.Application.Features.Hotels.GetHotels;
using HotelBooking.Application.Features.Hotels.UpdateHotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Features.Deals.GetFeaturedDeals;
using HotelBooking.Application.Features.Hotels.DeleteHotelImage;
using HotelBooking.Application.Features.Hotels.UploadHotelImage;
using HotelBooking.API.Extensions;

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

    private readonly UploadHotelImageService _uploadHotelImageService;
    private readonly DeleteHotelImageService _deleteHotelImageService;

    private readonly GetFeaturedDealsService _getFeaturedDealsService;

    public HotelsController(
        CreateHotelService createHotelService,
        GetHotelsService getHotelsService,
        GetHotelByIdService getHotelByIdService,
        DeleteHotelService deleteHotelService,
        UpdateHotelService updateHotelService,
        UploadHotelImageService uploadHotelImageService,
        DeleteHotelImageService deleteHotelImageService,
        GetFeaturedDealsService getFeaturedDealsService)
    {
        _createHotelService = createHotelService;
        _getHotelsService = getHotelsService;
        _getHotelByIdService = getHotelByIdService;
        _deleteHotelService = deleteHotelService;
        _updateHotelService = updateHotelService;

        _uploadHotelImageService = uploadHotelImageService;
        _deleteHotelImageService = deleteHotelImageService;

        _getFeaturedDealsService = getFeaturedDealsService;
    }

    [HttpGet("featured")]
    public async Task<ActionResult<List<GetFeaturedDealsResponse>>> GetFeatured(CancellationToken cancellationToken)
    {
        var result = await _getFeaturedDealsService.GetAsync(cancellationToken);

        return Ok(result);
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
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var hotels =
            await _getHotelsService.GetAllAsync(
                request,
                search,
                cancellationToken);


        return Ok(hotels);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetHotelByIdResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getHotelByIdService.GetAsync(id, cancellationToken);
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
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();


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

            if (result.Error == "You are not allowed to delete this hotel.")
            {
                return Forbid();
            }

            if (result.Error == "Cannot delete a hotel that has related data.")
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

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateHotelResponse>> Update(
        int id,
        UpdateHotelRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

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

    [Authorize]
    [HttpPost("{hotelId:int}/images")]
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

        var result =
            await _uploadHotelImageService.UploadAsync(
                hotelId,
                imageUpload,
                currentUserId,
                User.IsAdmin(),
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
                "You are not allowed to upload images for this hotel.")
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
    [HttpDelete("{hotelId:int}/images/{imageId:int}")]
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
            if (result.Error == "Hotel not found." || result.Error == "Hotel image not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to delete images for this hotel.")
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