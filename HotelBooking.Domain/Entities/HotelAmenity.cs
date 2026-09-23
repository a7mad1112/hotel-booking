using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class HotelAmenity : BaseEntity
{
    public int HotelId { get; set; }
    public int AmenityId { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}