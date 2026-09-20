namespace HotelBooking.Application.Features.Authentication.Register;

public sealed class RegisterResponse
{
    public int Id { get; init; }

    public string Email { get; init; } = string.Empty;
}