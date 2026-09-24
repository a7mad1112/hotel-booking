using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Payments.CreatePayment;

public sealed class CreatePaymentResponse
{
    public int BookingId { get; init; }

    public string Provider { get; init; } = string.Empty;

    public string TransactionId { get; init; } = string.Empty;

    public string? PaymentIntentId { get; init; }

    public decimal Amount { get; init; }

    public PaymentStatus Status { get; init; }

    public string CheckoutUrl { get; init; } = string.Empty;
}