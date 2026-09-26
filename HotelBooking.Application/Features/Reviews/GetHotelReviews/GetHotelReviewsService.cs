using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;

namespace HotelBooking.Application.Features.Reviews.GetHotelReviews;

public sealed class GetHotelReviewsService : IScopedService
{
    private readonly IReviewRepository _repository;

    public GetHotelReviewsService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetHotelReviewsResponse>> GetByHotelIdAsync(
        int hotelId,
        PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedByHotelIdAsync(
            hotelId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = result.Items
            .Select(review => new GetHotelReviewsResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                UserEmail = review.User?.Email ?? string.Empty,
                HotelId = review.HotelId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            })
            .ToList();

        return PagedResult<GetHotelReviewsResponse>.Create(items, result.TotalCount, request);
    }
}
