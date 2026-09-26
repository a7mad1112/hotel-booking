using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Common.Payments;

public sealed class PaymentGatewayFactory : IPaymentGatewayFactory, IScopedService
{
    private readonly IEnumerable<IPaymentGateway> _gateways;

    public PaymentGatewayFactory(IEnumerable<IPaymentGateway> gateways)
    {
        _gateways = gateways;
    }

    public IPaymentGateway GetGateway(string? providerName = null)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            return _gateways.FirstOrDefault(g => string.Equals(g.ProviderName, "Stripe", StringComparison.OrdinalIgnoreCase))
                ?? _gateways.FirstOrDefault()
                ?? throw new InvalidOperationException("No payment gateway is registered.");
        }

        var gateway = _gateways.FirstOrDefault(g => string.Equals(g.ProviderName, providerName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (gateway is null)
        {
            throw new NotSupportedException($"Payment provider '{providerName}' is not supported.");
        }

        return gateway;
    }

    public IReadOnlyCollection<string> GetSupportedProviders() =>
        _gateways.Select(g => g.ProviderName).ToList();
}
