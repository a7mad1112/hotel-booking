using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Cities.DeleteCity;

public sealed class DeleteCityService : IScopedService
{
    private readonly ICitiesRepository _citiesRepository;

    public DeleteCityService(
        ICitiesRepository citiesRepository)
    {
        _citiesRepository = citiesRepository;
    }

    public async Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var city = await _citiesRepository.GetByIdAsync(id, cancellationToken);

        if (city is null)
        {
            return Result.Failure(
                "City not found.");
        }

        var hasHotels =
            await _citiesRepository.HasHotelsAsync(id, cancellationToken);

        if (hasHotels)
        {
            return Result.Failure(
                "Cannot delete a city that has hotels.");
        }

        await _citiesRepository.DeleteAsync(city, cancellationToken);

        await _citiesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}