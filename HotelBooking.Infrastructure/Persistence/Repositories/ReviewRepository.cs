using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Reviews;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository : Repository<Review>, IReviewRepository, IScopedService
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(List<Review> Items, int TotalCount)> GetPagedByHotelIdAsync(
        int hotelId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.HotelId == hotelId);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToPagedListAsync(page, pageSize, cancellationToken);
    }

    public async Task<bool> HasUserBookedHotelAsync(
        int userId,
        int hotelId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Bookings
            .AsNoTracking()
            .AnyAsync(b => b.UserId == userId &&
                           b.Room.HotelId == hotelId &&
                           (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed),
                      cancellationToken);
    }
}
