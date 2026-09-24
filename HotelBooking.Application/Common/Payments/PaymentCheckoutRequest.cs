namespace HotelBooking.Application.Common.Payments;

public sealed class PaymentCheckoutRequest
{
    public int BookingId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "usd";
    public string CustomerEmail { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SuccessUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
}