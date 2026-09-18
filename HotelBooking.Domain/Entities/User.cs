using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Hotel> Hotels { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}