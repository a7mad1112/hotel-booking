using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Amenities.UpdateAmenity;

public sealed class UpdateAmenityService : IScopedService
{
    private readonly IAmenityRepository _repository;

    public UpdateAmenityService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<UpdateAmenityResponse>> UpdateAsync(
        int id,
        UpdateAmenityRequest request,
        CancellationToken cancellationToken)
    {
        var amenity = await _repository.GetByIdAsync(id, cancellationToken);
        if (amenity is null)
        {
            return ResultOfT<UpdateAmenityResponse>.Failure("Amenity not found.");
        }

        var name = request.Name.Trim();

        var exists = await _repository.ExistsByNameAsync(name, id, cancellationToken);
        if (exists)
        {
            return ResultOfT<UpdateAmenityResponse>.Failure("An amenity with the same name already exists.");
        }

        amenity.Name = name;
        amenity.Description = request.Description?.Trim();

        _repository.Update(amenity);
        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<UpdateAmenityResponse>.Success(new UpdateAmenityResponse
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Description = amenity.Description
        });
    }
}
