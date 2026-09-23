using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Deals.DeleteDeal;

public sealed class DeleteDealService : IScopedService
{
    private readonly IDealRepository _repository;

    public DeleteDealService(IDealRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> DeleteAsync(
        int id,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var deal = await _repository.GetByIdAsync(id, cancellationToken);

        if (deal is null)
        {
            return Result.Failure("Deal not found.");
        }

        var hotel = await _repository.GetHotelAsync(deal.HotelId, cancellationToken);

        if (hotel is null)
        {
            return Result.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return Result.Failure("You are not allowed to manage deals for this hotel.");
        }

        _repository.Delete(deal);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}