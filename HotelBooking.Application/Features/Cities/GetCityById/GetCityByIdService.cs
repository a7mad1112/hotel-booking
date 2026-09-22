using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Cities;

namespace HotelBooking.Application.Features.Cities.GetCityById;

public sealed class GetCityByIdService : IScopedService
{
    private readonly ICitiesRepository _citiesRepository;

    public GetCityByIdService(
        ICitiesRepository citiesRepository)
    {
        _citiesRepository = citiesRepository;
    }


    public async Task<ResultOfT<GetCityByIdResponse>> GetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var city =
            await _citiesRepository.GetByIdAsync(
                id,
                cancellationToken);


        if (city is null)
        {
            return ResultOfT<GetCityByIdResponse>.Failure(
                "City not found.");
        }


        return ResultOfT<GetCityByIdResponse>.Success(
            new GetCityByIdResponse
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                PostalCode = city.PostalCode
            });
    }
}