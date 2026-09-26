namespace HotelBooking.Application.Common.Pricing;

public interface IPricingCalculator
{
    PriceBreakdown CalculatePrice(
        decimal pricePerNight,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal? discountPercentage = null);
}
