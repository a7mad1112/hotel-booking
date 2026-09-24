using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Deals;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class DealRepository : Repository<Deal>, IDealRepository, IScopedService
{
    public DealRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Deal> Items, int TotalCount)> GetPagedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Deals
            .AsNoTracking()
            .Include(x => x.Hotel);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.OrderBy(x => x.StartDate).ThenBy(x => x.Hotel.Name)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Deal?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext.Deals
            .AsNoTracking()
            .Include(x => x.Hotel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Hotel?> GetHotelAsync(int hotelId, CancellationToken cancellationToken)
    {
        return await DbContext.Hotels
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);
    }

    public async Task<bool> HasOverlappingDealAsync(
        int hotelId,
        DateTime startDate,
        DateTime endDate,
        int? excludedDealId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Deals
            .AnyAsync(
                x =>
                    x.HotelId == hotelId
                    && (!excludedDealId.HasValue
                        || x.Id != excludedDealId.Value)
                    && x.StartDate < endDate
                    && startDate < x.EndDate,
                cancellationToken);
    }

    public async Task<List<Deal>> GetFeaturedAsync(DateTime now, int limit, CancellationToken cancellationToken)
    {
        return await DbContext.Deals
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Hotel)
            .ThenInclude(x => x.Images)
            .Include(x => x.Hotel)
            .ThenInclude(x => x.Rooms)
            .ThenInclude(x => x.Bookings)
            .Where(x => x.StartDate <= now && x.EndDate >= now)
            .OrderByDescending(x => x.DiscountPercentage)
            .ThenBy(x => x.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Deal?> GetApplicableDealAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate,
        CancellationToken cancellationToken)
    {
        return await DbContext.Deals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.HotelId == hotelId &&
                    x.StartDate <= checkInDate &&
                    x.EndDate >= checkOutDate,
                cancellationToken);
    }
}