namespace HotelBooking.Application.Features.Reviews.GetHotelReviews;

public sealed class GetHotelReviewsResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public int HotelId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
