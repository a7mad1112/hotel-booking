using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Features.Cities.GetTrendingCities;

public sealed class GetTrendingCitiesService : IScopedService
{
    private const int TrendingCitiesCount = 5;

    private readonly ICitiesRepository _repository;

    public GetTrendingCitiesService(ICitiesRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetTrendingCitiesResponse>> GetAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetTrendingAsync(TrendingCitiesCount, cancellationToken);
    }
}