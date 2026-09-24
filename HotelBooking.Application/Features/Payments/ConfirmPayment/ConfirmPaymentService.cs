using HotelBooking.Application.Common.Email;
using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Payments;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Payments.ConfirmPayment;

public sealed class ConfirmPaymentService : IScopedService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceGenerator _invoiceGenerator;
    private readonly IEmailSender _emailSender;

    public ConfirmPaymentService(
        IPaymentRepository paymentRepository,
        IInvoiceGenerator invoiceGenerator,
        IEmailSender emailSender)
    {
        _paymentRepository = paymentRepository;
        _invoiceGenerator = invoiceGenerator;
        _emailSender = emailSender;
    }

    public async Task<Result> ConfirmAsync(string transactionId, string? paymentIntentId, decimal amount,
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

        if (payment.Status == PaymentStatus.Refunded)
        {
            return Result.Failure("Payment has already been refunded.");
        }

        if (amount != payment.Amount)
        {
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
        }
        else if (!string.IsNullOrWhiteSpace(paymentIntentId) &&
                 payment.PaymentIntentId != paymentIntentId)
        {
            payment.PaymentIntentId = paymentIntentId;

            await _paymentRepository.SaveChangesAsync(cancellationToken);
        }

        if (payment.ConfirmationEmailSentAt is not null)
        {
            return Result.Success();
        }

        var invoice =
            await _invoiceGenerator.GenerateAsync(payment.Booking, cancellationToken);

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