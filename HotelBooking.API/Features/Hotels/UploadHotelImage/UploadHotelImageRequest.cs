using Microsoft.AspNetCore.Http;

namespace HotelBooking.API.Features.Hotels.UploadHotelImage;

public sealed class UploadHotelImageRequest
{
    public IFormFile? Image { get; set; }
}