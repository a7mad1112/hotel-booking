using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Amenities.GetAmenityById;

public sealed class GetAmenityByIdService : IScopedService
{
    private readonly IAmenityRepository _repository;

    public GetAmenityByIdService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetAmenityByIdResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var amenity = await _repository.GetByIdAsync(id, cancellationToken);

        if (amenity is null)
        {
            return ResultOfT<GetAmenityByIdResponse>.Failure("Amenity not found.");
        }

        return ResultOfT<GetAmenityByIdResponse>.Success(new GetAmenityByIdResponse
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Description = amenity.Description,
            CreatedAt = amenity.CreatedAt,
            UpdatedAt = amenity.UpdatedAt
        });
    }
}
