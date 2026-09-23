using System.Security.Claims;
using HotelBooking.Application.Features.Users.GetBookingHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly GetBookingHistoryService _getBookingHistoryService;

    public UsersController(GetBookingHistoryService getBookingHistoryService)
    {
        _getBookingHistoryService = getBookingHistoryService;
    }

    [Authorize]
    [HttpGet("history")]
    public async Task<ActionResult<List<GetBookingHistoryResponse>>> GetHistory(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await _getBookingHistoryService.GetAsync(currentUserId, cancellationToken);

        return Ok(result);
    }
}