using System.Security.Claims;
using HotelBooking.API.Extensions;
using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.API.Services;

/// <summary>
/// Implementation of ICurrentUserService using ASP.NET Core IHttpContextAccessor and ClaimsPrincipal.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService, IScopedService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId =>
        User is not null && User.TryGetUserId(out var userId) ? userId : null;

    public bool IsAdmin =>
        User?.IsAdmin() ?? false;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public int GetRequiredUserId()
    {
        if (UserId is not { } id)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return id;
    }
}
