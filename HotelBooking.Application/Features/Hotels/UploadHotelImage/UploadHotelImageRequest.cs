using HotelBooking.Application.Common.Images;

namespace HotelBooking.Application.Features.Hotels.UploadHotelImage;

public sealed class UploadHotelImageRequest
{
    public required ImageUpload Image { get; init; }
}