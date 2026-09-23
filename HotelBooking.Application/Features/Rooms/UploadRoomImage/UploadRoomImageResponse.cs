namespace HotelBooking.Application.Features.Rooms.UploadRoomImage;

public sealed class UploadRoomImageResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}