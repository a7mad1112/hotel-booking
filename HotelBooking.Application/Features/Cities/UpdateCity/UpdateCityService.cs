using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Cities.UpdateCity;

public sealed class UpdateCityService : IScopedService
{
    private readonly ICitiesRepository _citiesRepository;

    public UpdateCityService(
        ICitiesRepository citiesRepository)
    {
        _citiesRepository = citiesRepository;
    }

    public async Task<ResultOfT<UpdateCityResponse>> UpdateAsync(int id,
        UpdateCityRequest request,
        CancellationToken cancellationToken)
    {
        var city =
            await _citiesRepository.GetByIdAsync(
                id, cancellationToken);

        if (city is null)
        {
            return ResultOfT<UpdateCityResponse>.Failure("City not found.");
        }

        var name = request.Name.Trim();
        var country = request.Country.Trim();

        var exists =
            await _citiesRepository.ExistsAsync(
                name,
                country,
                id,
                cancellationToken);

        if (exists)
        {
            return ResultOfT<UpdateCityResponse>.Failure(
                "A city with the same name and country already exists.");
        }

        city.Name = name;
        city.Country = country;
        city.PostalCode = request.PostalCode;

        await _citiesRepository.SaveChangesAsync(cancellationToken);

        return ResultOfT<UpdateCityResponse>.Success(
            new UpdateCityResponse
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                PostalCode = city.PostalCode
            });
    }
}