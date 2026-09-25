using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Reviews.CreateReview;

public sealed class CreateReviewService : IScopedService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IHotelRepository _hotelRepository;

    public CreateReviewService(
        IReviewRepository reviewRepository,
        IHotelRepository hotelRepository)
    {
        _reviewRepository = reviewRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<ResultOfT<CreateReviewResponse>> CreateAsync(
        int hotelId,
        CreateReviewRequest request,
        int currentUserId,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            return ResultOfT<CreateReviewResponse>.Failure("Hotel not found.");
        }

        var hasBooked = await _reviewRepository.HasUserBookedHotelAsync(
            currentUserId,
            hotelId,
            cancellationToken);

        if (!hasBooked)
        {
            return ResultOfT<CreateReviewResponse>.Failure("Only guests who have booked this hotel can submit a review.");
        }

        var review = new Review
        {
            HotelId = hotelId,
            UserId = currentUserId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim()
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreateReviewResponse>.Success(new CreateReviewResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            HotelId = review.HotelId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        });
    }
}
