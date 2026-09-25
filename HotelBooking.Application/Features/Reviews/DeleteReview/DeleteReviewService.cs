using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Reviews.DeleteReview;

public sealed class DeleteReviewService : IScopedService
{
    private readonly IReviewRepository _repository;

    public DeleteReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> DeleteAsync(
        int id,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(id, cancellationToken);
        if (review is null)
        {
            return Result.Failure("Review not found.");
        }

        if (review.UserId != currentUserId && !isAdmin)
        {
            return Result.Failure("You are not allowed to delete this review.");
        }

        _repository.Delete(review);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
