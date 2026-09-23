namespace HotelBooking.Application.Features.RoomTypes.GetRoomTypeById;

public sealed class GetRoomTypeByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}