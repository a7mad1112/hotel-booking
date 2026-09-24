using HotelBooking.Application.Features.Payments;
using HotelBooking.Application.Features.Payments.FailPayment;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Payments.FailPayment;

public class FailPaymentServiceTests
{
    [Fact]
    public async Task FailAsync_PendingPayment_MarksPaymentAsFailed()
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

        var service = new FailPaymentService(paymentRepository.Object);

        // Act
        var result = await service.FailAsync("cs_test_123", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(PaymentStatus.Failed, payment.Status);

        // A failed payment does not confirm or cancel the booking.
        Assert.Equal(BookingStatus.Pending, payment.Booking.Status);

        paymentRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}