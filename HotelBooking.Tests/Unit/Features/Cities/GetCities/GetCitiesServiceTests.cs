using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Cities;
using HotelBooking.Application.Features.Cities.GetCities;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Cities.GetCities;

public class GetCitiesServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsPagedCities()
    {
        // Arrange
        var repository = new Mock<ICitiesRepository>();

        repository
            .Setup(x => x.GetPagedAsync(
                1,
                10,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new List<City>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Amman",
                            Country = "Jordan",
                            PostalCode = "11181"
                        }
                    },
                    1
                ));


        var service = new GetCitiesService(
            repository.Object);


        var request = new PaginationRequest
        {
            Page = 1,
            PageSize = 10
        };


        // Act
        var result = await service.GetAllAsync(
            request,
            null,
            CancellationToken.None);


        // Assert
        Assert.Single(result.Items);

        Assert.Equal(
            1,
            result.TotalCount);

        Assert.Equal(
            1,
            result.Page);

        Assert.Equal(
            10,
            result.PageSize);


        Assert.Equal(
            "Amman",
            result.Items[0].Name);

        Assert.Equal(
            "Jordan",
            result.Items[0].Country);
    }
}