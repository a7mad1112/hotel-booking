using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Cities;

namespace HotelBooking.Application.Features.Cities.GetCities;

public sealed class GetCitiesService : IScopedService
{
    private readonly ICitiesRepository _repository;

    public GetCitiesService(ICitiesRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetCitiesResponse>> GetAllAsync(
        PaginationRequest request,
        string? search,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            search,
            cancellationToken);

        var items = result.Items
            .Select(city => new GetCitiesResponse
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                PostalCode = city.PostalCode,
                NumberOfHotels = city.Hotels?.Count ?? 0,
                CreatedAt = city.CreatedAt,
                UpdatedAt = city.UpdatedAt
            })
            .ToList();

        return PagedResult<GetCitiesResponse>.Create(items, result.TotalCount, request);
    }
}