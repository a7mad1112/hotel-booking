using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Features.Bookings.Checkout;
using HotelBooking.Application.Features.Bookings.CreateBooking;
using HotelBooking.Application.Features.Payments.CreatePayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly CreateBookingService _createBookingService;
    private readonly GetCheckoutService _getCheckoutService;
    private readonly CreatePaymentService _createPaymentService;

    public BookingsController(CreateBookingService createBookingService, GetCheckoutService getCheckoutService,
        CreatePaymentService createPaymentService)
    {
        _createBookingService = createBookingService;
        _getCheckoutService = getCheckoutService;
        _createPaymentService = createPaymentService;
    }

    [Authorize(Policy = AuthorizationPolicies.CreateBooking)]
    [HttpPost("{id:int}/payment")]
    public async Task<ActionResult<CreatePaymentResponse>> CreatePayment(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        if (!Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyHeader))
        {
            return BadRequest(new
            {
                message = "Idempotency-Key header is required."
            });
        }

        var idempotencyKey = idempotencyHeader.ToString();

        var successUrl = $"{Request.Scheme}://{Request.Host}/api/bookings/{id}/payment/success";

        var cancelUrl = $"{Request.Scheme}://{Request.Host}/api/bookings/{id}/payment/cancel";

        var result = await _createPaymentService.CreateAsync(
            id,
            currentUserId,
            idempotencyKey,
            successUrl,
            cancelUrl,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Booking not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "Booking is not available for payment." ||
                result.Error == "A payment already exists for this booking.")
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

        return Ok(result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.CreateBooking)]
    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _createBookingService.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Room not found.")
            {
                return NotFound(new
                {
                    message = result.Error
                });
            }

            if (result.Error == "Room is not available." ||
                result.Error == "Room is not available for the selected dates.")
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

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.CreateBooking)]
    [HttpGet("{id:int}/checkout")]
    public async Task<ActionResult<GetCheckoutResponse>> GetCheckout(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _getCheckoutService.GetAsync(id, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.Error
            });
        }

        return Ok(result.Value);
    }
}