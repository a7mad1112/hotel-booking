using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Bookings.Checkout;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Bookings.Checkout;

public class GetCheckoutServiceTests
{
    [Fact]
    public async Task GetAsync_ExistingBookingBelongingToUser_ReturnsCheckout()
    {
        // Arrange
        var repository = new Mock<IBookingRepository>();

        repository
            .Setup(x => x.GetCheckoutAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Booking
                {
                    Id = 1,
                    UserId = 10,
                    RoomId = 20,
                    CheckInDate = new DateTime(2026, 10, 10),
                    CheckOutDate = new DateTime(2026, 10, 13),
                    TotalPrice = 240m,
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
                        PricePerNight = 100m,

                        Hotel = new Hotel
                        {
                            Id = 5,
                            Name = "Grand Hotel",

                            City = new City
                            {
                                Id = 2,
                                Name = "Amman",
                                Country = "Jordan"
                            }
                        }
                    }
                });

        var service = new GetCheckoutService(repository.Object);

        // Act
        var result = await service.GetAsync(1, 10, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value.BookingId);

        Assert.Equal(10, result.Value.Customer.UserId);

        Assert.Equal("customer@example.com", result.Value.Customer.Email);

        Assert.Equal("Grand Hotel", result.Value.Summary.HotelName);

        Assert.Equal("Amman", result.Value.Summary.CityName);

        Assert.Equal("302", result.Value.Summary.RoomNumber);

        Assert.Equal(3, result.Value.Summary.Nights);

        Assert.Equal(BookingStatus.Pending, result.Value.Summary.Status);

        Assert.Equal(100m, result.Value.Calculation.PricePerNight);

        Assert.Equal(3, result.Value.Calculation.Nights);

        Assert.Equal(240m, result.Value.Calculation.TotalPrice);
    }

    [Fact]
    public async Task GetAsync_BookingDoesNotBelongToUser_ReturnsFailure()
    {
        // Arrange
        var repository = new Mock<IBookingRepository>();

        repository
            .Setup(x => x.GetCheckoutAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var service = new GetCheckoutService(repository.Object);

        // Act
        var result = await service.GetAsync(1, 10, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);

        Assert.Equal("Booking not found.", result.Error);
    }
}