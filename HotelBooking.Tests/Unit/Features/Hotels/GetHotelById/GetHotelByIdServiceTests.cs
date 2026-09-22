using HotelBooking.Application.Features.Hotels;
using HotelBooking.Application.Features.Hotels.GetHotelById;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Hotels.GetHotelById;

public class GetHotelByIdServiceTests
{
    [Fact]
    public async Task GetAsync_ExistingHotel_ReturnsHotel()
    {
        var repository = new Mock<IHotelRepository>();

        repository
            .Setup(x => x.GetDetailsByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Hotel
                {
                    Id = 1,
                    Name = "Grand Hotel",
                    Description = "A nice hotel",
                    StarRating = 5,
                    Location = "Downtown",
                    CityId = 2,
                    City = new City
                    {
                        Id = 2,
                        Name = "Amman",
                        Country = "Jordan"
                    },
                    OwnerId = 3,
                    Owner = new User
                    {
                        Id = 3,
                        Email = "owner@example.com"
                    }
                });


        var service = new GetHotelByIdService(
            repository.Object);


        var result =
            await service.GetAsync(
                1,
                CancellationToken.None);


        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            "Grand Hotel",
            result.Value.Name);

        Assert.Equal(
            "Amman",
            result.Value.CityName);

        Assert.Equal(
            "Jordan",
            result.Value.Country);

        Assert.Equal(
            "owner@example.com",
            result.Value.OwnerEmail);
    }


    [Fact]
    public async Task GetAsync_HotelDoesNotExist_ReturnsFailure()
    {
        var repository = new Mock<IHotelRepository>();

        repository
            .Setup(x => x.GetDetailsByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel?)null);


        var service = new GetHotelByIdService(
            repository.Object);


        var result =
            await service.GetAsync(
                1,
                CancellationToken.None);


        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Hotel not found.",
            result.Error);
    }
}