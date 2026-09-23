namespace HotelBooking.Application.Features.RoomTypes.UpdateRoomType;

public sealed class UpdateRoomTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}