namespace HotelBooking.Application.Features.RoomTypes.UpdateRoomType;

public sealed class UpdateRoomTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}