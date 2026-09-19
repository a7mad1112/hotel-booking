using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Deal : BaseEntity
{
    public int HotelId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Hotel Hotel { get; set; } = null!;
}