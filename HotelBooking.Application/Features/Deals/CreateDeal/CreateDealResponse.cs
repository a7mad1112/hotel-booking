namespace HotelBooking.Application.Features.Deals.CreateDeal;

public sealed class CreateDealResponse
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}