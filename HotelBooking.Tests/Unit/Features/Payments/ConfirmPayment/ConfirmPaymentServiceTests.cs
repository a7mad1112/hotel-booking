using HotelBooking.Application.Common.Email;
using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Features.Payments;
using HotelBooking.Application.Features.Payments.ConfirmPayment;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotelBooking.Tests.Unit.Features.Payments.ConfirmPayment;

public class ConfirmPaymentServiceTests
{
    [Fact]
    public async Task ConfirmAsync_PendingPayment_ConfirmsPaymentAndSendsConfirmationEmail()
    {
        // Arrange
        var paymentRepository = new Mock<IPaymentRepository>();
        var invoiceGenerator = new Mock<IInvoiceGenerator>();
        var emailSender = new Mock<IEmailSender>();

        var payment = new Payment
        {
            Id = 1,
            BookingId = 10,
            Provider = "Stripe",
            TransactionId = "cs_test_123",
            PaymentIntentId = "pi_test_123",
            IdempotencyKey = "payment-key",
            CheckoutUrl = "https://checkout.stripe.com/test",
            Amount = 300m,
            Status = PaymentStatus.Pending,
            PaymentDate = DateTimeOffset.UtcNow,

            Booking = new Booking
            {
                Id = 10,
                UserId = 5,
                Status = BookingStatus.Pending,

                User = new User
                {
                    Id = 5,
                    Email = "customer@example.com"
                },

                Room = new Room
                {
                    Id = 20,
                    RoomNumber = "302",

                    Hotel = new Hotel
                    {
                        Id = 1,
                        Name = "Grand Hotel",

                        City = new City
                        {
                            Id = 1,
                            Name = "Amman",
                            Country = "Jordan"
                        }
                    }
                }
            }
        };

        paymentRepository
            .Setup(x => x.GetByTransactionIdAsync(
                "cs_test_123",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        invoiceGenerator
            .Setup(x => x.GenerateAsync(
                payment.Booking,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new InvoiceResult
                {
                    FileName = "booking-10-invoice.pdf",
                    ContentType = "application/pdf",
                    Content = [1, 2, 3]
                });

        var service = new ConfirmPaymentService(
            paymentRepository.Object,
            invoiceGenerator.Object,
            emailSender.Object,
            NullLogger<ConfirmPaymentService>.Instance);

        // Act
        var result = await service.ConfirmAsync(
            "cs_test_123",
            "pi_test_123",
            300m,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(PaymentStatus.Success, payment.Status);

        Assert.Equal(
            BookingStatus.Confirmed,
            payment.Booking.Status);

        Assert.Equal(
            "pi_test_123",
            payment.PaymentIntentId);

        Assert.NotNull(payment.ConfirmationEmailSentAt);

        invoiceGenerator.Verify(
            x => x.GenerateAsync(
                payment.Booking,
                It.IsAny<CancellationToken>()),
            Times.Once);

        emailSender.Verify(
            x => x.SendAsync(
                It.Is<EmailMessage>(email =>
                    email.To == "customer@example.com" &&
                    email.Subject == "Booking #10 confirmed" &&
                    email.Attachment != null &&
                    email.Attachment.FileName == "booking-10-invoice.pdf" &&
                    email.Attachment.ContentType == "application/pdf" &&
                    email.Attachment.Content.SequenceEqual(new byte[] { 1, 2, 3 })),
                It.IsAny<CancellationToken>()),
            Times.Once);

        paymentRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}