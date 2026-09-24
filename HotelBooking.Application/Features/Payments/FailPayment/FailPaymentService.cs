using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Payments.FailPayment;

public sealed class FailPaymentService : IScopedService
{
    private readonly IPaymentRepository _paymentRepository;

    public FailPaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result> FailAsync(
        string transactionId,
        CancellationToken cancellationToken)
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
            return Result.Failure("A successful payment cannot be marked as failed.");
        }

        if (payment.Status == PaymentStatus.Refunded)
        {
            return Result.Failure("Payment has already been refunded.");
        }

        payment.Status = PaymentStatus.Failed;

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}