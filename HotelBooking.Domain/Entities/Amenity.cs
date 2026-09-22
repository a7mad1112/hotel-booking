using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Amenity : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<HotelAmenity> HotelAmenities { get; set; } = [];
}