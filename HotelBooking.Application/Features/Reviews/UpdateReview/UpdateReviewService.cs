using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Reviews.UpdateReview;

public sealed class UpdateReviewService : IScopedService
{
    private readonly IReviewRepository _repository;

    public UpdateReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<UpdateReviewResponse>> UpdateAsync(
        int id,
        UpdateReviewRequest request,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(id, cancellationToken);
        if (review is null)
        {
            return ResultOfT<UpdateReviewResponse>.Failure("Review not found.");
        }

        if (review.UserId != currentUserId && !isAdmin)
        {
            return ResultOfT<UpdateReviewResponse>.Failure("You are not allowed to update this review.");
        }

        review.Rating = request.Rating;
        review.Comment = request.Comment?.Trim();

        _repository.Update(review);
        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<UpdateReviewResponse>.Success(new UpdateReviewResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            HotelId = review.HotelId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        });
    }
}
