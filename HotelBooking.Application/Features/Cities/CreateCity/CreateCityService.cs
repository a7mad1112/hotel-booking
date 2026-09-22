using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Cities.CreateCity;

public sealed class CreateCityService
{
    private readonly ICitiesRepository _citiesRepository;

    public CreateCityService(
        ICitiesRepository citiesRepository)
    {
        _citiesRepository = citiesRepository;
    }

    public async Task<ResultOfT<CreateCityResponse>> CreateAsync(
        CreateCityRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var country = request.Country.Trim();

        var exists = await _citiesRepository.ExistsAsync(
            name,
            country,
            null,
            cancellationToken);

        if (exists)
        {
            return ResultOfT<CreateCityResponse>.Failure(
                "A city with the same name and country already exists.");
        }

        var city = new City
        {
            Name = name,
            Country = country,
            PostalCode = request.PostalCode
        };

        await _citiesRepository.AddAsync(
            city,
            cancellationToken);

        await _citiesRepository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreateCityResponse>.Success(
            new CreateCityResponse
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                PostalCode = city.PostalCode
            });
    }
}