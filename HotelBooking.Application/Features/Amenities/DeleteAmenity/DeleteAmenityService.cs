using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Amenities.DeleteAmenity;

public sealed class DeleteAmenityService : IScopedService
{
    private readonly IAmenityRepository _repository;

    public DeleteAmenityService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var amenity = await _repository.GetByIdAsync(id, cancellationToken);
        if (amenity is null)
        {
            return Result.Failure("Amenity not found.");
        }

        var hasHotels = await _repository.HasHotelsAsync(id, cancellationToken);
        if (hasHotels)
        {
            return Result.Failure("Cannot delete an amenity that is assigned to hotels.");
        }

        _repository.Delete(amenity);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
