using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.Features.Authentication.Register;

public sealed class RegisterRequest
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}