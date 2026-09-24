namespace HotelBooking.Application.Common.Payments;

public interface IPaymentProvider
{
    string Name { get; }

    Task<PaymentCheckoutResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request,
        CancellationToken cancellationToken);
}