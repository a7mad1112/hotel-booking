using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.GetTrendingCities;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class CitiesRepository : Repository<City>, ICitiesRepository
{
    public CitiesRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(List<City> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<City> query = DbContext.Cities
            .AsNoTracking()
            .Include(x => x.Hotels);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmed = search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{trimmed}%") ||
                                     EF.Functions.ILike(x.Country, $"%{trimmed}%"));
        }

        return await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Country)
            .ThenBy(x => x.Id)
            .ToPagedListAsync(page, pageSize, cancellationToken);
    }


    public async Task<bool> ExistsAsync(string name, string country, int? excludeId,
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

    public async Task<bool> HasHotelsAsync(int cityId, CancellationToken cancellationToken)
    {
        return await DbContext.Hotels.AnyAsync(x => x.CityId == cityId, cancellationToken);
    }

    public async Task<List<GetTrendingCitiesResponse>> GetTrendingAsync(int count, CancellationToken cancellationToken)
    {
        return await DbContext.Bookings
            .AsNoTracking()
            .GroupBy(x => new
            {
                x.Room.Hotel.City.Id,
                x.Room.Hotel.City.Name,
                x.Room.Hotel.City.Country
            })
            .Select(x => new GetTrendingCitiesResponse
            {
                Id = x.Key.Id,
                Name = x.Key.Name,
                Country = x.Key.Country,
                BookingCount = x.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}