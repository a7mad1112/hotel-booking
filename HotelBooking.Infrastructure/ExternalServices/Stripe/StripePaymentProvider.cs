using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Payments;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace HotelBooking.Infrastructure.ExternalServices.Stripe;

public sealed class StripePaymentProvider : IPaymentProvider, IScopedService
{
    private readonly StripeClient _client;

    public StripePaymentProvider(IOptions<StripeOptions> options)
    {
        _client = new StripeClient(options.Value.SecretKey);
    }

    public string Name => "Stripe";
    public string ProviderName => Name;

    public async Task<PaymentCheckoutResult> CreateCheckoutSessionAsync(
        PaymentCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var sessionOptions = new SessionCreateOptions
        {
            Mode = "payment",

            CustomerEmail = request.CustomerEmail,

            SuccessUrl = request.SuccessUrl,

            CancelUrl = request.CancelUrl,

            ClientReferenceId = request.BookingId.ToString(),

            Metadata = new Dictionary<string, string>
            {
                ["booking_id"] = request.BookingId.ToString()
            },

            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,

                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,

                        UnitAmount = (long)Math.Round(
                            request.Amount * 100m),

                        ProductData =
                            new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = request.Description
                            }
                    }
                }
            ]
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = request.IdempotencyKey
        };

        var sessionService = _client.V1.Checkout.Sessions;

        var session = await sessionService.CreateAsync(
            sessionOptions,
            requestOptions,
            cancellationToken);

        return new PaymentCheckoutResult
        {
            TransactionId = session.Id,
            PaymentIntentId = session.PaymentIntentId,
            CheckoutUrl = session.Url
        };
    }
}