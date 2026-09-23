using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Deals.CreateDeal;

public sealed class CreateDealService : IScopedService
{
    private readonly IDealRepository _repository;

    public CreateDealService(IDealRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<CreateDealResponse>> CreateAsync(
        CreateDealRequest request,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var hotel = await _repository.GetHotelAsync(request.HotelId, cancellationToken);

        if (hotel is null)
        {
            return ResultOfT<CreateDealResponse>.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return ResultOfT<CreateDealResponse>.Failure("You are not allowed to manage deals for this hotel.");
        }

        var hasOverlap = await _repository.HasOverlappingDealAsync(
            request.HotelId,
            request.StartDate,
            request.EndDate,
            null,
            cancellationToken);

        if (hasOverlap)
        {
            return ResultOfT<CreateDealResponse>.Failure("The hotel already has a deal during this period.");
        }

        var deal = new Deal
        {
            HotelId = request.HotelId,
            DiscountPercentage = request.DiscountPercentage,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        await _repository.AddAsync(deal, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreateDealResponse>.Success(
            new CreateDealResponse
            {
                Id = deal.Id,
                HotelId = deal.HotelId,
                DiscountPercentage = deal.DiscountPercentage,
                StartDate = deal.StartDate,
                EndDate = deal.EndDate
            });
    }
}