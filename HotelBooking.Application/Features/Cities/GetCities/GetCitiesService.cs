namespace HotelBooking.Application.Features.Cities.GetCities;

public sealed class GetCitiesService
{
    private readonly ICitiesRepository _citiesRepository;

    public GetCitiesService(
        ICitiesRepository citiesRepository)
    {
        _citiesRepository = citiesRepository;
    }

    public async Task<List<GetCitiesResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var cities =
            await _citiesRepository.GetAllAsync(cancellationToken);


        return cities
            .Select(city => new GetCitiesResponse
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                PostalCode = city.PostalCode
            })
            .ToList();
    }
}