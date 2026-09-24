namespace HotelBooking.Application.Features.Payments.CreatePayment;

public sealed class CreatePaymentRequest
{
    public string IdempotencyKey { get; init; } = string.Empty;
}