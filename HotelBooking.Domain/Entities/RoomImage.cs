using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class RoomImage : AuditableEntity
{
    public int RoomId { get; set; }
    public required string ImageUrl { get; set; }
    public required string PublicId { get; set; }

    public Room Room { get; set; } = null!;
}