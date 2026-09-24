using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Bookings;

namespace HotelBooking.Application.Features.Bookings.Checkout;

public sealed class GetCheckoutService : IScopedService
{
    private readonly IBookingRepository _repository;

    public GetCheckoutService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetCheckoutResponse>> GetAsync(int bookingId, int currentUserId,
        CancellationToken cancellationToken)
    {
        var booking = await _repository.GetCheckoutAsync(bookingId, currentUserId, cancellationToken);

        if (booking is null)
        {
            return ResultOfT<GetCheckoutResponse>.Failure("Booking not found.");
        }

        var nights = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;

        return ResultOfT<GetCheckoutResponse>.Success(
            new GetCheckoutResponse
            {
                BookingId = booking.Id,

                Customer = new CustomerInfo
                {
                    UserId = booking.UserId,
                    Email = booking.User.Email
                },

                Summary = new BookingSummary
                {
                    HotelName = booking.Room.Hotel.Name,
                    CityName = booking.Room.Hotel.City.Name,

                    RoomId = booking.RoomId,
                    RoomNumber = booking.Room.RoomNumber,

                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,

                    Nights = nights,

                    Status = booking.Status
                },

                Calculation = new BookingCalculation
                {
                    PricePerNight = booking.Room.PricePerNight,
                    Nights = nights,
                    TotalPrice = booking.TotalPrice
                }
            });
    }
}