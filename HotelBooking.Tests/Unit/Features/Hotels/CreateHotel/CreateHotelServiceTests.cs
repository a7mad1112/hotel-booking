using HotelBooking.Application.Features.Hotels;
using HotelBooking.Application.Features.Hotels.CreateHotel;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Hotels.CreateHotel;

public class CreateHotelServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidHotel_ReturnsSuccess()
    {
        var repository = new Mock<IHotelRepository>();

        repository
            .Setup(x => x.CityExistsAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        repository
            .Setup(x => x.OwnerExistsAsync(
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CreateHotelService(
            repository.Object);

        var request = new CreateHotelRequest
        {
            CityId = 1,
            OwnerId = 2,
            Name = "Grand Hotel",
            Description = "A nice hotel",
            StarRating = 5,
            Location = "Amman"
        };

        var result = await service.CreateAsync(
            request,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Grand Hotel", result.Value.Name);

        repository.Verify(
            x => x.AddAsync(
                It.Is<Hotel>(hotel =>
                    hotel.CityId == 1 &&
                    hotel.OwnerId == 2 &&
                    hotel.Name == "Grand Hotel" &&
                    hotel.StarRating == 5 &&
                    hotel.Location == "Amman"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CityDoesNotExist_ReturnsFailure()
    {
        var repository = new Mock<IHotelRepository>();

        repository
            .Setup(x => x.CityExistsAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new CreateHotelService(
            repository.Object);

        var request = new CreateHotelRequest
        {
            CityId = 1,
            OwnerId = 2,
            Name = "Grand Hotel",
            StarRating = 5,
            Location = "Amman"
        };

        var result = await service.CreateAsync(
            request,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "City not found.",
            result.Error);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<Hotel>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}