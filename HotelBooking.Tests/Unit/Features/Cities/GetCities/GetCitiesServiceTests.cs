using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.GetCities;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Cities.GetCities;

public class GetCitiesServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsCities()
    {
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new City
                {
                    Id = 1,
                    Name = "Amman",
                    Country = "Jordan"
                }
            ]);


        var service = new GetCitiesService(
            repository.Object);


        var result = await service.GetAllAsync(
            CancellationToken.None);


        Assert.Single(result);

        Assert.Equal(
            "Amman",
            result[0].Name);

        Assert.Equal(
            "Jordan",
            result[0].Country);
    }
}