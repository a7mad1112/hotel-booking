namespace HotelBooking.Application.Features.Hotels.GetHotelById;

public sealed class HotelReviewResponse
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public int UserId { get; set; }
}