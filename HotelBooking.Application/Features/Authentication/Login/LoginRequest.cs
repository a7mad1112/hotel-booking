namespace HotelBooking.Application.Features.Authentication.Login;

public sealed class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}