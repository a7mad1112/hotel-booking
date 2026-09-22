using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Hotel : BaseEntity
{
    public int CityId { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public decimal StarRating { get; set; }
    public string Location { get; set; } = string.Empty;

    public City City { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<HotelImage> Images { get; set; } = [];
    public ICollection<Deal> Deals { get; set; } = [];
    public ICollection<HotelAmenity> HotelAmenities { get; set; } = [];
}