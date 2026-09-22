using HotelBooking.Application.Features.Hotels;
using HotelBooking.Application.Features.Hotels.UpdateHotel;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Hotels.UpdateHotel;

public class UpdateHotelServiceTests
{
    [Fact]
    public async Task UpdateAsync_AdminCanUpdateAnyHotel()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            CityId = 1,
            Name = "Old Hotel",
            StarRating = 3,
            Location = "Old Location"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        repository
            .Setup(x => x.CityExistsAsync(
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var service =
            new UpdateHotelService(
                repository.Object);


        var request = new UpdateHotelRequest
        {
            CityId = 2,
            Name = "New Hotel",
            Description = "Updated description",
            StarRating = 5,
            Location = "New Location"
        };


        var result =
            await service.UpdateAsync(
                1,
                request,
                currentUserId: 99,
                isAdmin: true,
                CancellationToken.None);


        Assert.True(result.IsSuccess);

        Assert.Equal(
            "New Hotel",
            hotel.Name);

        Assert.Equal(
            2,
            hotel.CityId);

        Assert.Equal(
            5,
            hotel.StarRating);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task UpdateAsync_OwnerCanUpdateOwnHotel()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            CityId = 1,
            Name = "Old Hotel",
            StarRating = 3,
            Location = "Old Location"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        repository
            .Setup(x => x.CityExistsAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var service =
            new UpdateHotelService(
                repository.Object);


        var request = new UpdateHotelRequest
        {
            CityId = 1,
            Name = "Updated Hotel",
            StarRating = 4,
            Location = "Updated Location"
        };


        var result =
            await service.UpdateAsync(
                1,
                request,
                currentUserId: 10,
                isAdmin: false,
                CancellationToken.None);


        Assert.True(result.IsSuccess);

        Assert.Equal(
            "Updated Hotel",
            hotel.Name);

        Assert.Equal(
            4,
            hotel.StarRating);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task UpdateAsync_DifferentOwner_ReturnsFailure()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            CityId = 1,
            Name = "Old Hotel"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);


        var service =
            new UpdateHotelService(
                repository.Object);


        var request = new UpdateHotelRequest
        {
            CityId = 1,
            Name = "Updated Hotel",
            StarRating = 4,
            Location = "Updated Location"
        };


        var result =
            await service.UpdateAsync(
                1,
                request,
                currentUserId: 99,
                isAdmin: false,
                CancellationToken.None);


        Assert.False(result.IsSuccess);

        Assert.Equal(
            "You are not allowed to update this hotel.",
            result.Error);

        repository.Verify(
            x => x.CityExistsAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_CityDoesNotExist_ReturnsFailure()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            CityId = 1,
            Name = "Grand Hotel"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        repository
            .Setup(x => x.CityExistsAsync(
                99,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        var service =
            new UpdateHotelService(
                repository.Object);


        var request = new UpdateHotelRequest
        {
            CityId = 99,
            Name = "Grand Hotel",
            StarRating = 5,
            Location = "Amman"
        };


        var result =
            await service.UpdateAsync(
                1,
                request,
                currentUserId: 10,
                isAdmin: false,
                CancellationToken.None);


        Assert.False(result.IsSuccess);

        Assert.Equal(
            "City not found.",
            result.Error);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}