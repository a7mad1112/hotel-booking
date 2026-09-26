using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Common.Pricing;

public sealed class StandardPricingStrategy : IPricingStrategy, ITransientService
{
    public bool CanApply(decimal? discountPercentage) =>
        !discountPercentage.HasValue || discountPercentage.Value <= 0;

    public PriceBreakdown Calculate(decimal pricePerNight, int nights, decimal? discountPercentage)
    {
        var safeNights = nights < 1 ? 1 : nights;
        var subtotal = pricePerNight * safeNights;

        return new PriceBreakdown(
            Nights: safeNights,
            PricePerNight: pricePerNight,
            Subtotal: subtotal,
            DiscountPercentage: null,
            DiscountAmount: 0m,
            TotalPrice: subtotal);
    }
}
