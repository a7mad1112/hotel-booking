using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int HotelId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Nights { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
}