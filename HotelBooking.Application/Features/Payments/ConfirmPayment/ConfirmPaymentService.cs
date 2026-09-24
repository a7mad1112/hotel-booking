using HotelBooking.Application.Common.Email;
using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Payments;
using HotelBooking.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Application.Features.Payments.ConfirmPayment;

public sealed class ConfirmPaymentService : IScopedService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceGenerator _invoiceGenerator;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ConfirmPaymentService> _logger;

    public ConfirmPaymentService(
        IPaymentRepository paymentRepository,
        IInvoiceGenerator invoiceGenerator,
        IEmailSender emailSender,
        ILogger<ConfirmPaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _invoiceGenerator = invoiceGenerator;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Result> ConfirmAsync(string transactionId, string? paymentIntentId, decimal amount,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment confirmation. TransactionId {TransactionId}", transactionId);

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            _logger.LogWarning("Payment confirmation rejected because transaction ID was missing.");

            return Result.Failure("Transaction ID is required.");
        }

        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Payment confirmation failed because payment was not found. TransactionId {TransactionId}", transactionId);

            return Result.Failure("Payment not found.");
        }

        if (payment.Status == PaymentStatus.Refunded)
        {
            _logger.LogWarning("Payment confirmation rejected because payment was already refunded. BookingId {BookingId}", payment.BookingId);

            return Result.Failure("Payment has already been refunded.");
        }

        if (amount != payment.Amount)
        {
            _logger.LogError("Payment amount mismatch. BookingId {BookingId}, ExpectedAmount {ExpectedAmount}, ReceivedAmount {ReceivedAmount}",
                payment.BookingId,
                payment.Amount,
                amount);

            return Result.Failure("Payment amount does not match the booking amount.");
        }

        if (payment.Status != PaymentStatus.Success)
        {
            payment.Status = PaymentStatus.Success;

            if (!string.IsNullOrWhiteSpace(paymentIntentId))
            {
                payment.PaymentIntentId = paymentIntentId;
            }

            payment.Booking.Status = BookingStatus.Confirmed;

            await _paymentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment confirmed and booking status updated. BookingId {BookingId}, Amount {Amount}", payment.BookingId, payment.Amount);
        }
        else if (!string.IsNullOrWhiteSpace(paymentIntentId) && payment.PaymentIntentId != paymentIntentId)
        {
            payment.PaymentIntentId = paymentIntentId;

            await _paymentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment already confirmed; payment intent information was updated. BookingId {BookingId}", payment.BookingId);
        }

        if (payment.ConfirmationEmailSentAt is not null)
        {
            _logger.LogInformation("Booking confirmation email was already sent. BookingId {BookingId}", payment.BookingId);

            return Result.Success();
        }

        var invoice = await _invoiceGenerator.GenerateAsync(payment.Booking, cancellationToken);

        var email = new EmailMessage
        {
            To = payment.Booking.User.Email,
            Subject =
                $"Booking #{payment.Booking.Id} confirmed",
            Body = BuildConfirmationEmail(payment),
            Attachment = new EmailAttachment
            {
                FileName = invoice.FileName,
                ContentType = invoice.ContentType,
                Content = invoice.Content
            }
        };

        await _emailSender.SendAsync(email, cancellationToken);

        payment.ConfirmationEmailSentAt = DateTimeOffset.UtcNow;

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking confirmation email sent successfully. BookingId {BookingId}", payment.BookingId);

        return Result.Success();
    }

    private static string BuildConfirmationEmail(Domain.Entities.Payment payment)
    {
        var booking = payment.Booking;

        var nights = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;

        return $"""
                Hello {booking.User.Email},

                Your hotel booking has been confirmed.

                Booking number: #{booking.Id}

                Hotel: {booking.Room.Hotel.Name}
                City: {booking.Room.Hotel.City.Name}
                Room: {booking.Room.RoomNumber}

                Check-in: {booking.CheckInDate:yyyy-MM-dd}
                Check-out: {booking.CheckOutDate:yyyy-MM-dd}
                Nights: {nights}

                Payment status: Paid
                Total amount: {payment.Amount:F2}

                Your invoice is attached to this email.

                Thank you for choosing Hotel Booking.
                """;
    }
}