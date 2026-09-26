using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;

namespace HotelBooking.Application.Features.Amenities.GetAmenities;

public sealed class GetAmenitiesService : IScopedService
{
    private readonly IAmenityRepository _repository;

    public GetAmenitiesService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetAmenitiesResponse>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = result.Items
            .Select(amenity => new GetAmenitiesResponse
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Description = amenity.Description
            })
            .ToList();

        return PagedResult<GetAmenitiesResponse>.Create(items, result.TotalCount, request);
    }
}
