using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;

namespace HotelBooking.Application.Features.Deals.GetDeals;

public sealed class GetDealsService : IScopedService
{
    private readonly IDealRepository _repository;

    public GetDealsService(IDealRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetDealsResponse>> GetAllAsync(PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        var items = result.Items.Select(deal => new GetDealsResponse
            {
                Id = deal.Id,
                HotelId = deal.HotelId,
                HotelName = deal.Hotel.Name,
                DiscountPercentage = deal.DiscountPercentage,
                StartDate = deal.StartDate,
                EndDate = deal.EndDate
            })
            .ToList();

        return PagedResult<GetDealsResponse>.Create(items, result.TotalCount, request);
    }
}