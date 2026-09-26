namespace HotelBooking.Application.Common.Payments;

/// <summary>
/// Factory interface for dynamically resolving payment gateway strategies.
/// </summary>
public interface IPaymentGatewayFactory
{
    /// <summary>
    /// Resolves the payment gateway matching the given provider name, or the default gateway if null.
    /// </summary>
    IPaymentGateway GetGateway(string? providerName = null);

    /// <summary>
    /// Returns the list of registered payment provider names.
    /// </summary>
    IReadOnlyCollection<string> GetSupportedProviders();
}
