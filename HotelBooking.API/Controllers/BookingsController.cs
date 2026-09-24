using System.Security.Claims;
using HotelBooking.API.Authorization;
using HotelBooking.Application.Features.Bookings.Checkout;
using HotelBooking.Application.Features.Bookings.CreateBooking;
using HotelBooking.Application.Features.Payments.ConfirmPayment;
using HotelBooking.Application.Features.Payments.CreatePayment;
using HotelBooking.Application.Features.Payments.FailPayment;
using HotelBooking.Infrastructure.ExternalServices.Stripe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly CreateBookingService _createBookingService;
    private readonly GetCheckoutService _getCheckoutService;
    private readonly CreatePaymentService _createPaymentService;
    private readonly ConfirmPaymentService _confirmPaymentService;
    private readonly FailPaymentService _failPaymentService;
    private readonly StripeOptions _stripeOptions;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        CreateBookingService createBookingService,
        GetCheckoutService getCheckoutService,
        CreatePaymentService createPaymentService,
        ConfirmPaymentService confirmPaymentService,
        FailPaymentService failPaymentService,
        IOptions<StripeOptions> stripeOptions,
        ILogger<BookingsController> logger)
    {
        _createBookingService = createBookingService;
        _getCheckoutService = getCheckoutService;
        _createPaymentService = createPaymentService;
        _confirmPaymentService = confirmPaymentService;
        _failPaymentService = failPaymentService;
        _stripeOptions = stripeOptions.Value;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("/api/payments/stripe/webhook")]
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stripe webhook received.");

        var json = await new StreamReader(Request.Body).ReadToEndAsync(cancellationToken);

        if (!Request.Headers.TryGetValue("Stripe-Signature", out var signature))
        {
            _logger.LogWarning("Stripe webhook rejected because Stripe-Signature header was missing.");

            return BadRequest();
        }

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature.ToString(), _stripeOptions.WebhookSecret);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Stripe webhook signature validation failed.");

            return BadRequest();
        }

        _logger.LogInformation("Stripe webhook validated. EventType {EventType}, EventId {EventId}",
            stripeEvent.Type,
            stripeEvent.Id);

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
            case EventTypes.CheckoutSessionAsyncPaymentSucceeded:
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session is null)
                    {
                        _logger.LogWarning("Stripe webhook contained an invalid checkout session.");

                        return BadRequest();
                    }

                    if (session.PaymentStatus != "paid")
                    {
                        _logger.LogInformation("Stripe checkout session was received but payment status was not paid. EventType {EventType}", stripeEvent.Type);

                        return Ok();
                    }

                    var amount = session.AmountTotal.HasValue
                            ? session.AmountTotal.Value / 100m
                            : 0m;

                    var result = await _confirmPaymentService.ConfirmAsync(session.Id, session.PaymentIntentId, amount, cancellationToken);

                    if (!result.IsSuccess && result.Error == "Payment not found.")
                    {
                        _logger.LogWarning("Stripe payment confirmation failed because payment was not found.");

                        return NotFound();
                    }

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Stripe payment confirmation failed. Error: {Error}", result.Error);

                        return BadRequest();
                    }

                    _logger.LogInformation("Stripe payment confirmation processed successfully.");

                    break;
                }

            case EventTypes.CheckoutSessionAsyncPaymentFailed:
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session is null)
                    {
                        _logger.LogWarning("Stripe payment failure webhook contained an invalid checkout session.");

                        return BadRequest();
                    }

                    var result = await _failPaymentService.FailAsync(session.Id, cancellationToken);

                    if (!result.IsSuccess && result.Error == "Payment not found.")
                    {
                        _logger.LogWarning("Stripe payment failure processing failed because payment was not found.");

                        return NotFound();
                    }

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Stripe payment failure processing failed. Error: {Error}", result.Error);

                        return BadRequest();
                    }

                    _logger.LogInformation("Stripe payment failure processed successfully.");

                    break;
                }
        }

        return Ok();
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

        var result = await _createPaymentService.CreateAsync(id, currentUserId, idempotencyKey, successUrl, cancelUrl, cancellationToken);

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
    public async Task<ActionResult<CreateBookingResponse>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
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