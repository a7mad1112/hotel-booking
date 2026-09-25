using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Reviews.GetReviewById;

public sealed class GetReviewByIdService : IScopedService
{
    private readonly IReviewRepository _repository;

    public GetReviewByIdService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetReviewByIdResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(id, cancellationToken);
        if (review is null)
        {
            return ResultOfT<GetReviewByIdResponse>.Failure("Review not found.");
        }

        return ResultOfT<GetReviewByIdResponse>.Success(new GetReviewByIdResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            UserEmail = review.User?.Email ?? string.Empty,
            HotelId = review.HotelId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        });
    }
}
