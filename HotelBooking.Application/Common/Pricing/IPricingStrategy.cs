namespace HotelBooking.Application.Common.Pricing;

public interface IPricingStrategy
{
    bool CanApply(decimal? discountPercentage);
    PriceBreakdown Calculate(decimal pricePerNight, int nights, decimal? discountPercentage);
}
