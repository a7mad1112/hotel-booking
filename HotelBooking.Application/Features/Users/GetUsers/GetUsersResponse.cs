using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Users.GetUsers;

public sealed class GetUsersResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
