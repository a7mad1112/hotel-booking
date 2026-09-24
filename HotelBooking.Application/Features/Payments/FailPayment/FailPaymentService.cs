using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Application.Features.Payments.FailPayment;

public sealed class FailPaymentService : IScopedService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<FailPaymentService> _logger;

    public FailPaymentService(IPaymentRepository paymentRepository, ILogger<FailPaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Result> FailAsync(string transactionId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing failed payment. TransactionId {TransactionId}", transactionId);

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            _logger.LogWarning("Payment failure processing rejected because transaction ID was missing.");

            return Result.Failure("Transaction ID is required.");
        }

        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Payment failure processing failed because payment was not found. TransactionId {TransactionId}", transactionId);

            return Result.Failure("Payment not found.");
        }

        if (payment.Status == PaymentStatus.Success)
        {
            _logger.LogWarning("Successful payment cannot be marked as failed. BookingId {BookingId}", payment.BookingId);

            return Result.Failure("A successful payment cannot be marked as failed.");
        }

        if (payment.Status == PaymentStatus.Refunded)
        {
            _logger.LogWarning("Refunded payment cannot be marked as failed. BookingId {BookingId}", payment.BookingId);

            return Result.Failure("Payment has already been refunded.");
        }

        payment.Status = PaymentStatus.Failed;

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment marked as failed. BookingId {BookingId}", payment.BookingId);

        return Result.Success();
    }
}