namespace HotelBooking.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User User { get; set; }
    public Hotel Hotel { get; set; }
}