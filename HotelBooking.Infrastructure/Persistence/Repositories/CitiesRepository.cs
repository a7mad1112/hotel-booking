using HotelBooking.Application.Features.Cities;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public class CitiesRepository : ICitiesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CitiesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<City>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Cities
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Cities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string name, string country,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Cities.AnyAsync(
            x =>
                x.Name == name &&
                x.Country == country &&
                (!excludeId.HasValue ||
                 x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(City city, CancellationToken cancellationToken)
    {
        await _dbContext.Cities.AddAsync(city, cancellationToken);
    }

    public Task DeleteAsync(City city,
        CancellationToken cancellationToken)
    {
        _dbContext.Cities.Remove(city);
        return Task.CompletedTask;
    }

    public async Task<bool> HasHotelsAsync(int cityId, CancellationToken cancellationToken)
    {
        return await _dbContext.Hotels.AnyAsync(
            x => x.Id == cityId, cancellationToken);
    }


    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}