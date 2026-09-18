using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class HotelImage : BaseEntity
{
    public int HotelId { get; set; }
    public string ImageUrl { get; set; }
    public string PublicId { get; set; }
    public Hotel Hotel { get; set; }
}