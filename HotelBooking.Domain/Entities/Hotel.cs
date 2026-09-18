namespace HotelBooking.Domain.Entities;

public class Hotel
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public decimal StarRating { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public City City { get; set; }
    public User Owner { get; set; }
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<HotelImage> Images { get; set; } = [];
    public ICollection<Deal> Deals { get; set; } = [];
}