namespace HotelBooking.Application.Common.Payments;

public interface IPaymentProvider : IPaymentGateway
{
    string Name { get; }

    string IPaymentGateway.ProviderName => Name;
}