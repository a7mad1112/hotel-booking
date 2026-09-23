using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Cities.GetTrendingCities;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Cities;

public interface ICitiesRepository : IRepository<City>
{
    Task<(List<City> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string name, string country, int? excludeId, CancellationToken cancellationToken);

    Task<bool> HasHotelsAsync(int cityId, CancellationToken cancellationToken);

    Task<List<GetTrendingCitiesResponse>> GetTrendingAsync(int count, CancellationToken cancellationToken);
}