using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Cities;

namespace HotelBooking.Application.Features.Cities.GetCities;

public sealed class GetCitiesService : IScopedService
{
    private readonly ICitiesRepository _repository;


    public GetCitiesService(
        ICitiesRepository repository)
    {
        _repository = repository;
    }


    public async Task<PagedResult<GetCitiesResponse>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _repository.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);


        return new PagedResult<GetCitiesResponse>
        {
            Items = result.Items
                .Select(city => new GetCitiesResponse
                {
                    Id = city.Id,
                    Name = city.Name,
                    Country = city.Country,
                    PostalCode = city.PostalCode
                })
                .ToList(),

            Page = request.Page,

            PageSize = request.PageSize,

            TotalCount = result.TotalCount
        };
    }
}