using System.Security.Claims;
using HotelBooking.Domain.Enums;

namespace HotelBooking.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var userId) ? userId : null;
    }

    public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
    {
        var id = user.GetUserId();
        if (id.HasValue)
        {
            userId = id.Value;
            return true;
        }

        userId = 0;
        return false;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(nameof(UserRole.Admin));
    }
}
