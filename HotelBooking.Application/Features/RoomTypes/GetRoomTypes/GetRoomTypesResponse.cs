namespace HotelBooking.Application.Features.RoomTypes.GetRoomTypes;

public sealed class GetRoomTypesResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}