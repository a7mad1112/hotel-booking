using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Payments.ConfirmPayment;

public sealed class ConfirmPaymentService : IScopedService
{
    private readonly IPaymentRepository _paymentRepository;

    public ConfirmPaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result> ConfirmAsync(string transactionId, string? paymentIntentId, decimal amount, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            return Result.Failure("Transaction ID is required.");
        }

        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId, cancellationToken);

        if (payment is null)
        {
            return Result.Failure("Payment not found.");
        }

        if (payment.Status == PaymentStatus.Success)
        {
            return Result.Success();
        }

        if (payment.Status == PaymentStatus.Refunded)
        {
            return Result.Failure("Payment has already been refunded.");
        }

        if (amount != payment.Amount)
        {
            return Result.Failure("Payment amount does not match the booking amount.");
        }

        payment.Status = PaymentStatus.Success;

        if (!string.IsNullOrWhiteSpace(paymentIntentId))
        {
            payment.PaymentIntentId = paymentIntentId;
        }

        payment.Booking.Status = BookingStatus.Confirmed;

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}