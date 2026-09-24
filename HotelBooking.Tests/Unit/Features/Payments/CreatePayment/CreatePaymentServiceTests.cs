using HotelBooking.Application.Common.Payments;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Payments;
using HotelBooking.Application.Features.Payments.CreatePayment;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotelBooking.Tests.Unit.Features.Payments.CreatePayment;

public class CreatePaymentServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidBooking_CreatesPendingPayment()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var paymentRepository = new Mock<IPaymentRepository>();
        var paymentProvider = new Mock<IPaymentProvider>();

        paymentProvider
            .Setup(x => x.Name)
            .Returns("Stripe");

        var booking = new Booking
        {
            Id = 1,
            UserId = 10,
            RoomId = 20,
            CheckInDate = new DateTime(2026, 10, 10),
            CheckOutDate = new DateTime(2026, 10, 13),
            TotalPrice = 300m,
            Status = BookingStatus.Pending,

            User = new User
            {
                Id = 10,
                Email = "customer@example.com"
            },

            Room = new Room
            {
                Id = 20,
                RoomNumber = "302",

                Hotel = new Hotel
                {
                    Id = 5,
                    Name = "Grand Hotel"
                }
            }
        };

        bookingRepository
            .Setup(x => x.GetCheckoutAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        paymentRepository
            .Setup(x => x.GetByIdempotencyKeyAsync(
                "payment-key-123",
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        paymentRepository
            .Setup(x => x.GetByBookingIdAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        paymentProvider
            .Setup(x => x.CreateCheckoutSessionAsync(
                It.IsAny<PaymentCheckoutRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PaymentCheckoutResult
                {
                    TransactionId = "cs_test_123",
                    PaymentIntentId = "pi_test_123",
                    CheckoutUrl = "https://checkout.stripe.com/test"
                });

        var service = new CreatePaymentService(
            bookingRepository.Object,
            paymentRepository.Object,
            paymentProvider.Object,
            NullLogger<CreatePaymentService>.Instance);

        // Act
        var result = await service.CreateAsync(
            bookingId: 1,
            currentUserId: 10,
            idempotencyKey: "payment-key-123",
            successUrl: "https://example.com/success",
            cancelUrl: "https://example.com/cancel",
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value.BookingId);
        Assert.Equal("Stripe", result.Value.Provider);
        Assert.Equal("cs_test_123", result.Value.TransactionId);
        Assert.Equal("pi_test_123", result.Value.PaymentIntentId);
        Assert.Equal(300m, result.Value.Amount);
        Assert.Equal(
            PaymentStatus.Pending,
            result.Value.Status);
        Assert.Equal(
            "https://checkout.stripe.com/test",
            result.Value.CheckoutUrl);

        paymentProvider.Verify(
            x => x.CreateCheckoutSessionAsync(
                It.Is<PaymentCheckoutRequest>(request =>
                    request.BookingId == 1 &&
                    request.Amount == 300m &&
                    request.Currency == "usd" &&
                    request.CustomerEmail == "customer@example.com" &&
                    request.Description == "Hotel booking #1 - Grand Hotel" &&
                    request.SuccessUrl == "https://example.com/success" &&
                    request.CancelUrl == "https://example.com/cancel" &&
                    request.IdempotencyKey == "payment-key-123"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        paymentRepository.Verify(
            x => x.AddAsync(
                It.Is<Payment>(payment =>
                    payment.BookingId == 1 &&
                    payment.Provider == "Stripe" &&
                    payment.TransactionId == "cs_test_123" &&
                    payment.PaymentIntentId == "pi_test_123" &&
                    payment.IdempotencyKey == "payment-key-123" &&
                    payment.CheckoutUrl == "https://checkout.stripe.com/test" &&
                    payment.Amount == 300m &&
                    payment.Status == PaymentStatus.Pending),
                It.IsAny<CancellationToken>()),
            Times.Once);

        paymentRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ExistingIdempotencyKey_ReturnsExistingPayment()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var paymentRepository = new Mock<IPaymentRepository>();
        var paymentProvider = new Mock<IPaymentProvider>();

        var existingPayment = new Payment
        {
            Id = 50,
            BookingId = 1,
            Provider = "Stripe",
            TransactionId = "cs_existing",
            PaymentIntentId = "pi_existing",
            IdempotencyKey = "payment-key-123",
            CheckoutUrl = "https://checkout.stripe.com/existing",
            Amount = 300m,
            Status = PaymentStatus.Pending,
            PaymentDate = DateTimeOffset.UtcNow,

            Booking = new Booking
            {
                Id = 1,
                UserId = 10
            }
        };

        paymentRepository
            .Setup(x => x.GetByIdempotencyKeyAsync(
                "payment-key-123",
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPayment);

        var service = new CreatePaymentService(
            bookingRepository.Object,
            paymentRepository.Object,
            paymentProvider.Object,
            NullLogger<CreatePaymentService>.Instance);

        // Act
        var result = await service.CreateAsync(
            bookingId: 1,
            currentUserId: 10,
            idempotencyKey: "payment-key-123",
            successUrl: "https://example.com/success",
            cancelUrl: "https://example.com/cancel",
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value.BookingId);
        Assert.Equal("Stripe", result.Value.Provider);
        Assert.Equal("cs_existing", result.Value.TransactionId);
        Assert.Equal("pi_existing", result.Value.PaymentIntentId);
        Assert.Equal(300m, result.Value.Amount);
        Assert.Equal(
            PaymentStatus.Pending,
            result.Value.Status);
        Assert.Equal(
            "https://checkout.stripe.com/existing",
            result.Value.CheckoutUrl);

        paymentProvider.Verify(
            x => x.CreateCheckoutSessionAsync(
                It.IsAny<PaymentCheckoutRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        paymentRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Payment>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        paymentRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}