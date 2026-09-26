using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Common.Pricing;

public sealed class PricingCalculator : IPricingCalculator, IScopedService
{
    private readonly IEnumerable<IPricingStrategy> _strategies;

    public PricingCalculator()
        : this([new DealDiscountPricingStrategy(), new StandardPricingStrategy()])
    {
    }

    public PricingCalculator(IEnumerable<IPricingStrategy> strategies)
    {
        _strategies = strategies;
    }

    public PriceBreakdown CalculatePrice(
        decimal pricePerNight,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal? discountPercentage = null)
    {
        var nights = Math.Max(1, (checkOutDate.Date - checkInDate.Date).Days);

        var strategy = _strategies.FirstOrDefault(s => s.CanApply(discountPercentage))
            ?? new StandardPricingStrategy();

        return strategy.Calculate(pricePerNight, nights, discountPercentage);
    }
}
