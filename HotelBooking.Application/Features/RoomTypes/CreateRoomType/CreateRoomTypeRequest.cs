namespace HotelBooking.Application.Features.RoomTypes.CreateRoomType;

public sealed class CreateRoomTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}