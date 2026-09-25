namespace HotelBooking.Application.Features.Reviews.UpdateReview;

public sealed class UpdateReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
