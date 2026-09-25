namespace HotelBooking.Application.Features.Reviews.CreateReview;

public sealed class CreateReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
