using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Payments;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, int userId, CancellationToken cancellationToken);

    Task<Payment?> GetByBookingIdAsync(int bookingId, int userId, CancellationToken cancellationToken);
}