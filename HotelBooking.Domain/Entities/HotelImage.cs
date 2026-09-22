using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class HotelImage : AuditableEntity
{
    public int HotelId { get; set; }
    public required string ImageUrl { get; set; }
    public required string PublicId { get; set; }
    public Hotel Hotel { get; set; } = null!;
}