namespace HotelBooking.Domain.Entities;

public class HotelImage
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string ImageUrl { get; set; }
    public string PublicId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Hotel Hotel { get; set; }
}