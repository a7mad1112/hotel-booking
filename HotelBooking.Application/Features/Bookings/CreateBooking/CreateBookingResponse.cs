using HotelBooking.Domain.Enums;

public sealed class CreateBookingResponse
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int HotelId { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public int Nights { get; set; }

    public decimal PricePerNight { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; }

    public string? SpecialRequests { get; set; }
}