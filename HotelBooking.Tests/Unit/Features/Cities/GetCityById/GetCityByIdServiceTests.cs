using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.GetCityById;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Cities.GetCityById;

public class GetCityByIdServiceTests
{
    [Fact]
    public async Task GetAsync_ExistingCity_ReturnsCity()
    {
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new City
                {
                    Id = 1,
                    Name = "Amman",
                    Country = "Jordan"
                });


        var service =
            new GetCityByIdService(
                repository.Object);


        var result =
            await service.GetAsync(
                1,
                CancellationToken.None);


        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);

        Assert.Equal(
            "Amman",
            result.Value!.Name);

        Assert.Equal(
            "Jordan",
            result.Value.Country);
    }

    [Fact]
    public async Task GetAsync_NonExistingCity_ReturnsFailure()
    {
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((City?)null);


        var service =
            new GetCityByIdService(
                repository.Object);


        var result =
            await service.GetAsync(
                1,
                CancellationToken.None);


        Assert.False(result.IsSuccess);

        Assert.Equal(
            "City not found.",
            result.Error);
    }
}