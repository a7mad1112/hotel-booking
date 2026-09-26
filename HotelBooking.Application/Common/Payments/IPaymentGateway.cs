namespace HotelBooking.Application.Common.Payments;

/// <summary>
/// Strategy interface for pluggable payment gateways (Stripe, Mock, PayPal, etc.).
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Unique identifier / name of the payment provider (e.g. "Stripe", "Mock").
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Creates a checkout session with the payment provider.
    /// </summary>
    Task<PaymentCheckoutResult> CreateCheckoutSessionAsync(
        PaymentCheckoutRequest request,
        CancellationToken cancellationToken);
}
