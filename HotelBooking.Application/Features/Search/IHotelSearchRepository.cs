using HotelBooking.Application.Features.Search.Hotels;

namespace HotelBooking.Application.Features.Search;

public interface IHotelSearchRepository
{
    Task<(List<SearchHotelsResponse> Items, int TotalCount)> SearchAsync(
        SearchHotelsRequest request,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}