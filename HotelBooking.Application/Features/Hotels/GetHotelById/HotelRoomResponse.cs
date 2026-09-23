namespace HotelBooking.Application.Features.Hotels.GetHotelById;

public sealed class HotelRoomResponse
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;

    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public string? RoomTypeDescription { get; set; }

    public decimal PricePerNight { get; set; }
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public bool Availability { get; set; }

    public List<RoomImageResponse> Images { get; set; } = [];
}