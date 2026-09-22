namespace HotelBooking.Application.Features.Hotels.UploadHotelImage;

public sealed class UploadHotelImageResponse
{
    public int Id { get; init; }
    public int HotelId { get; init; }
    public required string ImageUrl { get; init; }
}