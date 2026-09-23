namespace HotelBooking.Application.Features.Deals.CreateDeal;

public sealed class CreateDealRequest
{
    public int HotelId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}