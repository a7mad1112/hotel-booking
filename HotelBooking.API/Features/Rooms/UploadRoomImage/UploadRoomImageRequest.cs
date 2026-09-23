namespace HotelBooking.API.Features.Rooms.UploadRoomImage;

public sealed class UploadRoomImageRequest
{
    public required IFormFile Image { get; set; }
}