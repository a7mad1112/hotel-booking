using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;

namespace HotelBooking.Application.Features.Search.Hotels;

public sealed class SearchHotelsService : IScopedService
{
    private readonly IHotelSearchRepository _repository;

    public SearchHotelsService(IHotelSearchRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SearchHotelsResponse>> SearchAsync(
        SearchHotelsRequest request,
        PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var result = await _repository.SearchAsync(
            request,
            pagination.Page,
            pagination.PageSize,
            cancellationToken);

        return new PagedResult<SearchHotelsResponse>
        {
            Items = result.Items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = result.TotalCount
        };
    }
}