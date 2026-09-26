namespace HotelBooking.Application.Common.Interfaces;

/// <summary>
/// Abstraction for accessing current authenticated user information without coupling to HttpContext.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The ID of the currently authenticated user, or null if unauthenticated.
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Indicates whether the current user has the Admin role.
    /// </summary>
    bool IsAdmin { get; }

    /// <summary>
    /// Indicates whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Returns the UserId, or throws UnauthorizedAccessException if not authenticated.
    /// </summary>
    int GetRequiredUserId();
}
