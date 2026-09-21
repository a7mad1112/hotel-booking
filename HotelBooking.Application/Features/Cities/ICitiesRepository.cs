using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Cities;

public interface ICitiesRepository
{
    Task<List<City>> GetAllAsync(CancellationToken cancellationToken);

    Task<City?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string name,
        string country,
        int? excludeId,
        CancellationToken cancellationToken);

    Task AddAsync(City city, CancellationToken cancellationToken);

    Task DeleteAsync(City city, CancellationToken cancellationToken);

    Task<bool> HasHotelsAsync(int cityId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}