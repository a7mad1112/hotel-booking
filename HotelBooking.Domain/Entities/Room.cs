using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Room : BaseEntity
{
    public int HotelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public bool Availability { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = [];
}