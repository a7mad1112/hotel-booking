using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Bookings.CreateBooking;
using HotelBooking.Application.Features.Deals;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotelBooking.Tests.Unit.Features.Bookings.CreateBooking;

public class CreateBookingServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidBooking_CalculatesPriceAndCreatesBooking()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var dealRepository = new Mock<IDealRepository>();

        var room = new Room
        {
            Id = 1,
            HotelId = 10,
            RoomNumber = "101",
            PricePerNight = 100m,
            Availability = true
        };

        roomRepository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        bookingRepository
            .Setup(x => x.HasOverlappingBookingAsync(
                1,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        dealRepository
            .Setup(x => x.GetApplicableDealAsync(
                10,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Deal?)null);

        var service = new CreateBookingService(bookingRepository.Object, roomRepository.Object, dealRepository.Object,
              NullLogger<CreateBookingService>.Instance);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            CheckInDate = new DateTime(2026, 10, 1),
            CheckOutDate = new DateTime(2026, 10, 4),
            SpecialRequests = "Quiet room on top floor"
        };

        // Act
        var result = await service.CreateAsync(request, currentUserId: 5, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value.RoomId);
        Assert.Equal(10, result.Value.HotelId);
        Assert.Equal(3, result.Value.Nights);
        Assert.Equal(100m, result.Value.PricePerNight);

        Assert.Equal(300m, result.Value.Subtotal);
        Assert.Null(result.Value.DiscountPercentage);
        Assert.Equal(0m, result.Value.DiscountAmount);
        Assert.Equal(300m, result.Value.TotalPrice);

        Assert.Equal(BookingStatus.Pending, result.Value.Status);
        Assert.Equal("Quiet room on top floor", result.Value.SpecialRequests);

        bookingRepository.Verify(
            x => x.AddAsync(
                It.Is<Booking>(booking =>
                    booking.UserId == 5 &&
                    booking.RoomId == 1 &&
                    booking.CheckInDate == request.CheckInDate &&
                    booking.CheckOutDate == request.CheckOutDate &&
                    booking.TotalPrice == 300m &&
                    booking.Status == BookingStatus.Pending &&
                    booking.SpecialRequests == "Quiet room on top floor"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        bookingRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_OverlappingBooking_ReturnsFailure()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var dealRepository = new Mock<IDealRepository>();

        var room = new Room
        {
            Id = 1,
            HotelId = 10,
            RoomNumber = "101",
            PricePerNight = 100m,
            Availability = true
        };

        roomRepository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        bookingRepository
            .Setup(x => x.HasOverlappingBookingAsync(
                1,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CreateBookingService(
            bookingRepository.Object,
            roomRepository.Object,
            dealRepository.Object,
            NullLogger<CreateBookingService>.Instance);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            CheckInDate = new DateTime(2026, 10, 1),
            CheckOutDate = new DateTime(2026, 10, 4)
        };

        // Act
        var result = await service.CreateAsync(request, currentUserId: 5, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        Assert.Equal("Room is not available for the selected dates.", result.Error);

        bookingRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Booking>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        bookingRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        dealRepository.Verify(
            x => x.GetApplicableDealAsync(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_OverlappingBookingConcurrencyRace_ReturnsFailure()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var dealRepository = new Mock<IDealRepository>();

        var room = new Room
        {
            Id = 1,
            HotelId = 10,
            RoomNumber = "101",
            PricePerNight = 100m,
            Availability = true
        };

        roomRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        bookingRepository
            .Setup(x => x.HasOverlappingBookingAsync(
                1,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        dealRepository
            .Setup(x => x.GetApplicableDealAsync(
                10,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Deal?)null);

        bookingRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OverlappingBookingException());

        var service = new CreateBookingService(
            bookingRepository.Object,
            roomRepository.Object,
            dealRepository.Object,
            NullLogger<CreateBookingService>.Instance);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            CheckInDate = new DateTime(2026, 10, 1),
            CheckOutDate = new DateTime(2026, 10, 4)
        };

        // Act
        var result = await service.CreateAsync(request, currentUserId: 5, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Room is not available for the selected dates.", result.Error);
    }
}