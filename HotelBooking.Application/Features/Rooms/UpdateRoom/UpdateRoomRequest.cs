namespace HotelBooking.Application.Features.Rooms.UpdateRoom;

public sealed class UpdateRoomRequest
{
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public bool Availability { get; set; }
}