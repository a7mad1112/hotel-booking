using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Review : BaseEntity
{
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public User User { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
}