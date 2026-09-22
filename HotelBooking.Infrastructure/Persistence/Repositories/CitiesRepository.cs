using HotelBooking.Application.Features.Cities;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class CitiesRepository
    : Repository<City>, ICitiesRepository
{
    public CitiesRepository(
        ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }


    public async Task<(List<City> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Cities
            .AsNoTracking();


        var totalCount =
            await query.CountAsync(
                cancellationToken);


        var items =
            await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Country)
                .ThenBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


        return (
            items,
            totalCount);
    }


    public async Task<bool> ExistsAsync(
        string name,
        string country,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Cities.AnyAsync(
            x =>
                x.Name == name &&
                x.Country == country &&
                (!excludeId.HasValue ||
                 x.Id != excludeId.Value),
            cancellationToken);
    }


    public async Task<bool> HasHotelsAsync(
        int cityId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Hotels.AnyAsync(
            x => x.CityId == cityId,
            cancellationToken);
    }
}