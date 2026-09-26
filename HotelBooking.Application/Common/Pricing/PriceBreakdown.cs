namespace HotelBooking.Application.Common.Pricing;

public sealed record PriceBreakdown(
    int Nights,
    decimal PricePerNight,
    decimal Subtotal,
    decimal? DiscountPercentage,
    decimal DiscountAmount,
    decimal TotalPrice);
