namespace HotelBooking.Application.Features.Deals.UpdateDeal;

public sealed class UpdateDealRequest
{
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}