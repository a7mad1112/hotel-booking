namespace HotelBooking.Application.Common.Payments;

public sealed class PaymentCheckoutResult
{
    public string TransactionId { get; init; } = string.Empty;
    public string? PaymentIntentId { get; init; }
    public string CheckoutUrl { get; init; } = string.Empty;
}