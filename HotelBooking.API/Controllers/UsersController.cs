using System.Security.Claims;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Users.GetBookingHistory;
using HotelBooking.Application.Features.Users.GetUsers;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly GetBookingHistoryService _getBookingHistoryService;
    private readonly GetUsersService _getUsersService;

    public UsersController(
        GetBookingHistoryService getBookingHistoryService,
        GetUsersService getUsersService)
    {
        _getBookingHistoryService = getBookingHistoryService;
        _getUsersService = getUsersService;
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

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetUsersResponse>>> GetUsers(
        [FromQuery] PaginationRequest request,
        [FromQuery] string? search,
        [FromQuery] UserRole? role,
        CancellationToken cancellationToken)
    {
        var result = await _getUsersService.GetUsersAsync(request, search, role, cancellationToken);
        return Ok(result);
    }
}