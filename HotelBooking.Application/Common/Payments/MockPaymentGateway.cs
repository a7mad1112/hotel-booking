using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Common.Payments;

/// <summary>
/// Mock payment gateway strategy for testing and development environments.
/// </summary>
public sealed class MockPaymentGateway : IPaymentGateway, IScopedService
{
    public string ProviderName => "Mock";

    public Task<PaymentCheckoutResult> CreateCheckoutSessionAsync(
        PaymentCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = new PaymentCheckoutResult
        {
            TransactionId = $"mock_tx_{Guid.NewGuid():N}",
            PaymentIntentId = $"mock_pi_{Guid.NewGuid():N}",
            CheckoutUrl = $"{request.SuccessUrl}?mock_session=success&booking_id={request.BookingId}"
        };

        return Task.FromResult(result);
    }
}
