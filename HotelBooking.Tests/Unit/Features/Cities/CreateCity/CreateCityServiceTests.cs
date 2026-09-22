using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.CreateCity;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Cities.CreateCity;

public class CreateCityServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidCity_ReturnsSuccess()
    {
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.ExistsAsync(
                "Amman",
                "Jordan",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        var service = new CreateCityService(
            repository.Object);


        var request = new CreateCityRequest
        {
            Name = "Amman",
            Country = "Jordan"
        };


        var result = await service.CreateAsync(
            request,
            CancellationToken.None);


        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<City>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCity_ReturnsFailure()
    {
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.ExistsAsync(
                "Amman",
                "Jordan",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var service = new CreateCityService(
            repository.Object);


        var request = new CreateCityRequest
        {
            Name = "Amman",
            Country = "Jordan"
        };


        var result = await service.CreateAsync(
            request,
            CancellationToken.None);


        Assert.False(result.IsSuccess);

        Assert.Equal(
            "A city with the same name and country already exists.",
            result.Error);


        repository.Verify(
            x => x.AddAsync(
                It.IsAny<City>(),
                It.IsAny<CancellationToken>()),
            Times.Never);


        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}