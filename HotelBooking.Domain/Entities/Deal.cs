namespace HotelBooking.Domain.Entities;

public class Deal
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Hotel Hotel { get; set; }
}