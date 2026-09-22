using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class HotelRepository
    : Repository<Hotel>, IHotelRepository, IScopedService
{
    public HotelRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }


    public async Task<(List<Hotel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Hotels
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.Owner);


        var totalCount =
            await query.CountAsync(
                cancellationToken);


        var items =
            await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.City.Name)
                .ThenBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


        return (
            items,
            totalCount);
    }


    public async Task<Hotel?> GetDetailsByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await DbContext.Hotels
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }


    public async Task<bool> CityExistsAsync(
        int cityId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Cities
            .AnyAsync(
                x => x.Id == cityId,
                cancellationToken);
    }


    public async Task<bool> OwnerExistsAsync(
        int ownerId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Users
            .AnyAsync(
                x => x.Id == ownerId,
                cancellationToken);
    }
}