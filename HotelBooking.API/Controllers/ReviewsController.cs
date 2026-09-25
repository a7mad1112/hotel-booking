using System.Security.Claims;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Reviews.CreateReview;
using HotelBooking.Application.Features.Reviews.DeleteReview;
using HotelBooking.Application.Features.Reviews.GetHotelReviews;
using HotelBooking.Application.Features.Reviews.GetReviewById;
using HotelBooking.Application.Features.Reviews.UpdateReview;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.API.Extensions;

namespace HotelBooking.API.Controllers;

[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly GetHotelReviewsService _getHotelReviewsService;
    private readonly GetReviewByIdService _getReviewByIdService;
    private readonly CreateReviewService _createReviewService;
    private readonly UpdateReviewService _updateReviewService;
    private readonly DeleteReviewService _deleteReviewService;

    public ReviewsController(
        GetHotelReviewsService getHotelReviewsService,
        GetReviewByIdService getReviewByIdService,
        CreateReviewService createReviewService,
        UpdateReviewService updateReviewService,
        DeleteReviewService deleteReviewService)
    {
        _getHotelReviewsService = getHotelReviewsService;
        _getReviewByIdService = getReviewByIdService;
        _createReviewService = createReviewService;
        _updateReviewService = updateReviewService;
        _deleteReviewService = deleteReviewService;
    }

    [HttpGet("api/hotels/{hotelId:int}/reviews")]
    public async Task<ActionResult<PagedResult<GetHotelReviewsResponse>>> GetByHotelId(
        int hotelId,
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _getHotelReviewsService.GetByHotelIdAsync(hotelId, request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/reviews/{id:int}")]
    public async Task<ActionResult<GetReviewByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _getReviewByIdService.GetAsync(id, cancellationToken);

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
    [HttpPost("api/hotels/{hotelId:int}/reviews")]
    public async Task<ActionResult<CreateReviewResponse>> Create(
        int hotelId,
        CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _createReviewService.CreateAsync(
            hotelId,
            request,
            currentUserId,
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

            if (result.Error == "Only guests who have booked this hotel can submit a review.")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = result.Error
                });
            }

            return BadRequest(new
            {
                message = result.Error
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [Authorize]
    [HttpPut("api/reviews/{id:int}")]
    public async Task<ActionResult<UpdateReviewResponse>> Update(
        int id,
        UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

        var result = await _updateReviewService.UpdateAsync(
            id,
            request,
            currentUserId,
            isAdmin,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Review not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to update this review.")
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
    [HttpDelete("api/reviews/{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

        var result = await _deleteReviewService.DeleteAsync(
            id,
            currentUserId,
            isAdmin,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Review not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "You are not allowed to delete this review.")
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
