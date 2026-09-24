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

    public async Task<Booking?> GetCheckoutAsync(int bookingId, int userId, CancellationToken cancellationToken)
    {
        return await DbContext.Bookings
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Room)
            .ThenInclude(x => x.Hotel)
            .ThenInclude(x => x.City)
            .FirstOrDefaultAsync(x => x.Id == bookingId && x.UserId == userId,
                cancellationToken);
    }
}