using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Deals.GetDealById;

public sealed class GetDealByIdService : IScopedService
{
    private readonly IDealRepository _repository;

    public GetDealByIdService(IDealRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetDealByIdResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var deal = await _repository.GetDetailsByIdAsync(id, cancellationToken);

        if (deal is null)
        {
            return ResultOfT<GetDealByIdResponse>.Failure("Deal not found.");
        }

        return ResultOfT<GetDealByIdResponse>.Success(
            new GetDealByIdResponse
            {
                Id = deal.Id,
                HotelId = deal.HotelId,
                HotelName = deal.Hotel.Name,
                DiscountPercentage = deal.DiscountPercentage,
                StartDate = deal.StartDate,
                EndDate = deal.EndDate
            });
    }
}