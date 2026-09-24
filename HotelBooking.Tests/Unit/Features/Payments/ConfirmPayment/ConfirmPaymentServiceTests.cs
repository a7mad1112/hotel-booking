using HotelBooking.Application.Features.Payments;
using HotelBooking.Application.Features.Payments.ConfirmPayment;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Payments.ConfirmPayment;

public class ConfirmPaymentServiceTests
{
    [Fact]
    public async Task ConfirmAsync_PendingPayment_ConfirmsPaymentAndBooking()
    {
        // Arrange
        var paymentRepository = new Mock<IPaymentRepository>();

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
                Status = BookingStatus.Pending
            }
        };

        paymentRepository
            .Setup(x => x.GetByTransactionIdAsync(
                "cs_test_123",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var service = new ConfirmPaymentService(paymentRepository.Object);

        // Act
        var result = await service.ConfirmAsync(
            "cs_test_123",
            "pi_test_123",
            300m,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(PaymentStatus.Success, payment.Status);

        Assert.Equal(BookingStatus.Confirmed, payment.Booking.Status);

        Assert.Equal("pi_test_123", payment.PaymentIntentId);

        paymentRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}