using HotelBooking.API.Authorization;
using HotelBooking.API.Extensions;
using HotelBooking.Application.Features.Bookings.Checkout;
using HotelBooking.Application.Features.Bookings.CreateBooking;
using HotelBooking.Application.Features.Bookings.GetBookingInvoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly CreateBookingService _createBookingService;
    private readonly GetCheckoutService _getCheckoutService;
    private readonly GetBookingInvoiceService _getBookingInvoiceService;

    public BookingsController(
        CreateBookingService createBookingService,
        GetCheckoutService getCheckoutService,
        GetBookingInvoiceService getBookingInvoiceService)
    {
        _createBookingService = createBookingService;
        _getCheckoutService = getCheckoutService;
        _getBookingInvoiceService = getBookingInvoiceService;
    }

    [Authorize(Policy = AuthorizationPolicies.CreateBooking)]
    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _createBookingService.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [Authorize(Policy = AuthorizationPolicies.CreateBooking)]
    [HttpGet("{id:int}/checkout")]
    public async Task<ActionResult<GetCheckoutResponse>> GetCheckout(int id, CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _getCheckoutService.GetAsync(id, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetInvoice(int id, CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsAdmin();

        var result = await _getBookingInvoiceService.GetInvoiceAsync(id, currentUserId, isAdmin, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToErrorResult();
        }

        return File(result.Value!.Content, result.Value.ContentType, result.Value.FileName);
    }
}