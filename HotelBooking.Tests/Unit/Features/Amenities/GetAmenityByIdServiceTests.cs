using HotelBooking.Application.Features.Amenities;
using HotelBooking.Application.Features.Amenities.GetAmenityById;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Amenities;

public class GetAmenityByIdServiceTests
{
    [Fact]
    public async Task GetAsync_ExistingAmenity_ReturnsSuccess()
    {
        var repository = new Mock<IAmenityRepository>();
        var amenity = new Amenity { Id = 1, Name = "Gym", Description = "Fitness equipment" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        var service = new GetAmenityByIdService(repository.Object);

        var result = await service.GetAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
        Assert.Equal("Gym", result.Value.Name);
    }

    [Fact]
    public async Task GetAsync_AmenityNotFound_ReturnsFailure()
    {
        var repository = new Mock<IAmenityRepository>();
        repository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity?)null);

        var service = new GetAmenityByIdService(repository.Object);

        var result = await service.GetAsync(99, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Amenity not found.", result.Error);
    }
}
