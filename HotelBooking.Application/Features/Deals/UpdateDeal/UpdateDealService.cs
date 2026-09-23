using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Deals.UpdateDeal;

public sealed class UpdateDealService : IScopedService
{
    private readonly IDealRepository _repository;

    public UpdateDealService(IDealRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<UpdateDealResponse>> UpdateAsync(
        int id,
        UpdateDealRequest request,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var deal = await _repository.GetByIdAsync(id, cancellationToken);

        if (deal is null)
        {
            return ResultOfT<UpdateDealResponse>.Failure("Deal not found.");
        }

        var hotel =
            await _repository.GetHotelAsync(deal.HotelId, cancellationToken);

        if (hotel is null)
        {
            return ResultOfT<UpdateDealResponse>.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return ResultOfT<UpdateDealResponse>.Failure("You are not allowed to manage deals for this hotel.");
        }

        var hasOverlap = await _repository.HasOverlappingDealAsync(
            deal.HotelId,
            request.StartDate,
            request.EndDate,
            deal.Id,
            cancellationToken);

        if (hasOverlap)
        {
            return ResultOfT<UpdateDealResponse>.Failure("The hotel already has a deal during this period.");
        }

        deal.DiscountPercentage = request.DiscountPercentage;
        deal.StartDate = request.StartDate;
        deal.EndDate = request.EndDate;

        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<UpdateDealResponse>.Success(
            new UpdateDealResponse
            {
                Id = deal.Id,
                HotelId = deal.HotelId,
                DiscountPercentage = deal.DiscountPercentage,
                StartDate = deal.StartDate,
                EndDate = deal.EndDate
            });
    }
}