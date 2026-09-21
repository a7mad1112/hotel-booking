namespace HotelBooking.Application.Features.Authentication.Login;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; init; }
}