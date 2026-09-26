using HotelBooking.Application.Common.Payments;
using Moq;

namespace HotelBooking.Tests.Unit.Common.Payments;

public class PaymentGatewayFactoryTests
{
    [Fact]
    public void GetGateway_WhenProviderNameIsNull_ReturnsDefaultStripeGateway()
    {
        // Arrange
        var stripeGateway = new Mock<IPaymentGateway>();
        stripeGateway.Setup(g => g.ProviderName).Returns("Stripe");

        var mockGateway = new Mock<IPaymentGateway>();
        mockGateway.Setup(g => g.ProviderName).Returns("Mock");

        var factory = new PaymentGatewayFactory([stripeGateway.Object, mockGateway.Object]);

        // Act
        var result = factory.GetGateway(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Stripe", result.ProviderName);
    }

    [Fact]
    public void GetGateway_WhenProviderNameIsMock_ReturnsMockGateway()
    {
        // Arrange
        var stripeGateway = new Mock<IPaymentGateway>();
        stripeGateway.Setup(g => g.ProviderName).Returns("Stripe");

        var mockGateway = new Mock<IPaymentGateway>();
        mockGateway.Setup(g => g.ProviderName).Returns("Mock");

        var factory = new PaymentGatewayFactory([stripeGateway.Object, mockGateway.Object]);

        // Act
        var result = factory.GetGateway("mock");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mock", result.ProviderName);
    }

    [Fact]
    public void GetGateway_WhenProviderIsUnsupported_ThrowsNotSupportedException()
    {
        // Arrange
        var stripeGateway = new Mock<IPaymentGateway>();
        stripeGateway.Setup(g => g.ProviderName).Returns("Stripe");

        var factory = new PaymentGatewayFactory([stripeGateway.Object]);

        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => factory.GetGateway("Crypto"));
        Assert.Contains("Crypto", ex.Message);
    }

    [Fact]
    public void GetSupportedProviders_ReturnsAllRegisteredNames()
    {
        // Arrange
        var stripeGateway = new Mock<IPaymentGateway>();
        stripeGateway.Setup(g => g.ProviderName).Returns("Stripe");

        var mockGateway = new Mock<IPaymentGateway>();
        mockGateway.Setup(g => g.ProviderName).Returns("Mock");

        var factory = new PaymentGatewayFactory([stripeGateway.Object, mockGateway.Object]);

        // Act
        var providers = factory.GetSupportedProviders();

        // Assert
        Assert.Equal(2, providers.Count);
        Assert.Contains("Stripe", providers);
        Assert.Contains("Mock", providers);
    }

    [Fact]
    public async Task MockPaymentGateway_CreatesValidMockCheckoutSession()
    {
        // Arrange
        var gateway = new MockPaymentGateway();
        var request = new PaymentCheckoutRequest
        {
            BookingId = 42,
            Amount = 150m,
            Currency = "usd",
            CustomerEmail = "test@example.com",
            Description = "Test Booking",
            SuccessUrl = "https://example.com/success",
            CancelUrl = "https://example.com/cancel",
            IdempotencyKey = "key-123"
        };

        // Act
        var result = await gateway.CreateCheckoutSessionAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("mock_tx_", result.TransactionId);
        Assert.StartsWith("mock_pi_", result.PaymentIntentId);
        Assert.Contains("booking_id=42", result.CheckoutUrl);
    }
}
