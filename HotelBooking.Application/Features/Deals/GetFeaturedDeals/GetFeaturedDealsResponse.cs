namespace HotelBooking.Application.Features.Deals.GetFeaturedDeals;

public sealed class GetFeaturedDealsResponse
{
    public int HotelId { get; set; }
    public string? HotelImageUrl { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal Rating { get; set; }
}