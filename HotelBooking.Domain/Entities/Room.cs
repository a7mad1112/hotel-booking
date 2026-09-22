using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Room : AuditableEntity
{
    public int HotelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public decimal PricePerNight { get; set; }
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public bool Availability { get; set; }

    public RoomType RoomType { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<RoomImage> Images { get; set; } = [];
}