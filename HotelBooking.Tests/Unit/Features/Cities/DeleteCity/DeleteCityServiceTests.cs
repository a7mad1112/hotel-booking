using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.DeleteCity;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Cities.DeleteCity;

public class DeleteCityServiceTests
{
    [Fact]
    public async Task DeleteAsync_EmptyCity_ReturnsSuccess()
    {
        var repository = new Mock<ICitiesRepository>();

        var city = new City
        {
            Id = 1,
            Name = "Amman"
        };


        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);


        repository
            .Setup(x => x.HasHotelsAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        var service = new DeleteCityService(
            repository.Object);


        var result = await service.DeleteAsync(
            1,
            CancellationToken.None);


        Assert.True(result.IsSuccess);


        repository.Verify(
            x => x.Delete(city),
            Times.Once);


        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_CityHasHotels_ReturnsFailure()
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
            .Setup(x => x.HasHotelsAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var service = new DeleteCityService(
            repository.Object);


        var result = await service.DeleteAsync(
            1,
            CancellationToken.None);


        Assert.False(result.IsSuccess);


        repository.Verify(
            x => x.Delete(
                It.IsAny<City>()),
            Times.Never);


        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}