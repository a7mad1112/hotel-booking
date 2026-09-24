namespace HotelBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingRequest
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}