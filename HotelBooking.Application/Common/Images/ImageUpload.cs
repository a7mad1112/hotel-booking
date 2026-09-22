namespace HotelBooking.Application.Common.Images;

public sealed class ImageUpload
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
}