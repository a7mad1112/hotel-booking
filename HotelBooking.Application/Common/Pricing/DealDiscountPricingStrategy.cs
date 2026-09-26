using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Common.Pricing;

public sealed class DealDiscountPricingStrategy : IPricingStrategy, ITransientService
{
    public bool CanApply(decimal? discountPercentage) =>
        discountPercentage.HasValue && discountPercentage.Value > 0;

    public PriceBreakdown Calculate(decimal pricePerNight, int nights, decimal? discountPercentage)
    {
        var safeNights = nights < 1 ? 1 : nights;
        var subtotal = pricePerNight * safeNights;
        var discountPct = discountPercentage!.Value;
        var discountAmount = Math.Round(subtotal * discountPct / 100m, 2, MidpointRounding.AwayFromZero);
        var totalPrice = Math.Max(0m, subtotal - discountAmount);

        return new PriceBreakdown(
            Nights: safeNights,
            PricePerNight: pricePerNight,
            Subtotal: subtotal,
            DiscountPercentage: discountPct,
            DiscountAmount: discountAmount,
            TotalPrice: totalPrice);
    }
}
