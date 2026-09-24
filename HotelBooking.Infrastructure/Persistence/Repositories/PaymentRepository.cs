using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Payments;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository : Repository<Payment>, IPaymentRepository, IScopedService
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, int userId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(
                x =>
                    x.IdempotencyKey == idempotencyKey &&
                    x.Booking.UserId == userId,
                cancellationToken);
    }

    public async Task<Payment?> GetByBookingIdAsync(int bookingId, int userId, CancellationToken cancellationToken)
    {
        return await DbContext.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(
                x =>
                    x.BookingId == bookingId &&
                    x.Booking.UserId == userId,
                cancellationToken);
    }
}