namespace HotelBooking.Application.Features.Deals.UpdateDeal;

public sealed class UpdateDealResponse
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}