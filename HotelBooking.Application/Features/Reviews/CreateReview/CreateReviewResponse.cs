namespace HotelBooking.Application.Features.Reviews.CreateReview;

public sealed class CreateReviewResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
