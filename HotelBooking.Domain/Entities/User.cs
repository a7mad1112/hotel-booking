using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public ICollection<Hotel> Hotels { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}