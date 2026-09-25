using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Amenities.CreateAmenity;

public sealed class CreateAmenityService : IScopedService
{
    private readonly IAmenityRepository _repository;

    public CreateAmenityService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<CreateAmenityResponse>> CreateAsync(
        CreateAmenityRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _repository.ExistsByNameAsync(name, null, cancellationToken);
        if (exists)
        {
            return ResultOfT<CreateAmenityResponse>.Failure("An amenity with the same name already exists.");
        }

        var amenity = new Amenity
        {
            Name = name,
            Description = request.Description?.Trim()
        };

        await _repository.AddAsync(amenity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreateAmenityResponse>.Success(new CreateAmenityResponse
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Description = amenity.Description
        });
    }
}
