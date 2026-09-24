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
        // Arrange
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
                    },

                    Images = [],
                    Rooms = [],
                    Reviews = []
                });

        var service = new GetHotelByIdService(repository.Object);

        // Act
        var result = await service.GetAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value.Id);

        Assert.Equal("Grand Hotel", result.Value.Name);

        Assert.Equal("A nice hotel", result.Value.Description);

        Assert.Equal(5, result.Value.StarRating);

        Assert.Equal("Downtown", result.Value.Location);

        Assert.Equal(2, result.Value.CityId);

        Assert.Equal("Amman", result.Value.CityName);

        Assert.Equal("Jordan", result.Value.Country);

        Assert.Equal(3, result.Value.OwnerId);

        Assert.Equal("owner@example.com", result.Value.OwnerEmail);

        Assert.Empty(result.Value.Images);
        Assert.Empty(result.Value.Rooms);
        Assert.Empty(result.Value.Reviews);
    }
}