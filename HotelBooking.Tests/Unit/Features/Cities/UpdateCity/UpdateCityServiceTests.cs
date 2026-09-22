using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.UpdateCity;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Cities.UpdateCity;

public class UpdateCityServiceTests
{
    [Fact]
    public async Task UpdateAsync_ValidCity_UpdatesCity()
    {
        var repository = new Mock<ICitiesRepository>();

        var city = new City
        {
            Id = 1,
            Name = "Old Name",
            Country = "Jordan"
        };


        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);


        repository
            .Setup(x => x.ExistsAsync(
                "Amman",
                "Jordan",
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        var service = new UpdateCityService(
            repository.Object);


        var request = new UpdateCityRequest
        {
            Name = "Amman",
            Country = "Jordan"
        };


        var result = await service.UpdateAsync(
            1,
            request,
            CancellationToken.None);


        Assert.True(result.IsSuccess);

        Assert.Equal(
            "Amman",
            city.Name);


        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateCity_ReturnsFailure()
    {
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new City
                {
                    Id = 1
                });


        repository
            .Setup(x => x.ExistsAsync(
                "Amman",
                "Jordan",
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var service = new UpdateCityService(
            repository.Object);


        var result = await service.UpdateAsync(
            1,
            new UpdateCityRequest
            {
                Name = "Amman",
                Country = "Jordan"
            },
            CancellationToken.None);


        Assert.False(result.IsSuccess);


        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}