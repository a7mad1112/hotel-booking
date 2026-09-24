using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository
    : Repository<Booking>, IBookingRepository, IScopedService
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasOverlappingBookingAsync(
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        CancellationToken cancellationToken)
    {
        return await DbContext.Bookings
            .AnyAsync(
                booking =>
                    booking.RoomId == roomId &&
                    (booking.Status == BookingStatus.Pending ||
                     booking.Status == BookingStatus.Confirmed) &&
                    booking.CheckInDate < checkOutDate &&
                    checkInDate < booking.CheckOutDate,
                cancellationToken);
    }
}