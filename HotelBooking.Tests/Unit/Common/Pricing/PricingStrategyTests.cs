using HotelBooking.Application.Common.Pricing;

namespace HotelBooking.Tests.Unit.Common.Pricing;

public class PricingStrategyTests
{
    [Fact]
    public void StandardPricingStrategy_CalculatesCorrectPrice_WithZeroDiscount()
    {
        // Arrange
        var strategy = new StandardPricingStrategy();

        // Act
        var result = strategy.Calculate(pricePerNight: 100m, nights: 3, discountPercentage: null);

        // Assert
        Assert.Equal(3, result.Nights);
        Assert.Equal(100m, result.PricePerNight);
        Assert.Equal(300m, result.Subtotal);
        Assert.Null(result.DiscountPercentage);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(300m, result.TotalPrice);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(-5, true)]
    [InlineData(10, false)]
    public void StandardPricingStrategy_CanApply_EvaluatesDiscountPercentage(int? discount, bool expected)
    {
        var strategy = new StandardPricingStrategy();
        Assert.Equal(expected, strategy.CanApply(discount));
    }

    [Fact]
    public void DealDiscountPricingStrategy_CalculatesCorrectDiscountAndTotal()
    {
        // Arrange
        var strategy = new DealDiscountPricingStrategy();

        // Act (e.g. $100/night for 4 nights = $400, 25% discount = $100 off, total = $300)
        var result = strategy.Calculate(pricePerNight: 100m, nights: 4, discountPercentage: 25m);

        // Assert
        Assert.Equal(4, result.Nights);
        Assert.Equal(100m, result.PricePerNight);
        Assert.Equal(400m, result.Subtotal);
        Assert.Equal(25m, result.DiscountPercentage);
        Assert.Equal(100m, result.DiscountAmount);
        Assert.Equal(300m, result.TotalPrice);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(0, false)]
    [InlineData(-10, false)]
    [InlineData(15, true)]
    public void DealDiscountPricingStrategy_CanApply_EvaluatesDiscountPercentage(int? discount, bool expected)
    {
        var strategy = new DealDiscountPricingStrategy();
        Assert.Equal(expected, strategy.CanApply(discount));
    }

    [Fact]
    public void PricingCalculator_WithoutDeal_UsesStandardStrategy()
    {
        // Arrange
        var calculator = new PricingCalculator();
        var checkIn = new DateTime(2026, 10, 1);
        var checkOut = new DateTime(2026, 10, 4); // 3 nights

        // Act
        var result = calculator.CalculatePrice(pricePerNight: 150m, checkIn, checkOut, discountPercentage: null);

        // Assert
        Assert.Equal(3, result.Nights);
        Assert.Equal(450m, result.Subtotal);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(450m, result.TotalPrice);
    }

    [Fact]
    public void PricingCalculator_WithDeal_UsesDealDiscountStrategy()
    {
        // Arrange
        var calculator = new PricingCalculator();
        var checkIn = new DateTime(2026, 10, 1);
        var checkOut = new DateTime(2026, 10, 3); // 2 nights

        // Act ($200/night * 2 = $400, 20% discount = $80, total = $320)
        var result = calculator.CalculatePrice(pricePerNight: 200m, checkIn, checkOut, discountPercentage: 20m);

        // Assert
        Assert.Equal(2, result.Nights);
        Assert.Equal(400m, result.Subtotal);
        Assert.Equal(20m, result.DiscountPercentage);
        Assert.Equal(80m, result.DiscountAmount);
        Assert.Equal(320m, result.TotalPrice);
    }
}
