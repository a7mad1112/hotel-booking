using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Bookings;

public interface IBookingRepository : IRepository<Booking>
{
    Task<bool> HasOverlappingBookingAsync(int roomId, DateTime checkInDate, DateTime checkOutDate,
        CancellationToken cancellationToken);

    Task<Booking?> GetCheckoutAsync(int bookingId, int userId, CancellationToken cancellationToken);

    Task<Booking?> GetBookingForInvoiceAsync(int bookingId, CancellationToken cancellationToken);
}