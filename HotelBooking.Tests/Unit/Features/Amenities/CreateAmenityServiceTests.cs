using HotelBooking.Application.Features.Amenities;
using HotelBooking.Application.Features.Amenities.CreateAmenity;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Amenities;

public class CreateAmenityServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidAmenity_ReturnsSuccess()
    {
        var repository = new Mock<IAmenityRepository>();
        repository
            .Setup(x => x.ExistsByNameAsync("Free Wi-Fi", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new CreateAmenityService(repository.Object);

        var request = new CreateAmenityRequest
        {
            Name = "Free Wi-Fi",
            Description = "High-speed internet access"
        };

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Free Wi-Fi", result.Value!.Name);
        Assert.Equal("High-speed internet access", result.Value.Description);

        repository.Verify(x => x.AddAsync(It.Is<Amenity>(a => a.Name == "Free Wi-Fi"), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateName_ReturnsFailure()
    {
        var repository = new Mock<IAmenityRepository>();
        repository
            .Setup(x => x.ExistsByNameAsync("Free Wi-Fi", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CreateAmenityService(repository.Object);

        var request = new CreateAmenityRequest
        {
            Name = "Free Wi-Fi",
            Description = "High-speed internet access"
        };

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("An amenity with the same name already exists.", result.Error);

        repository.Verify(x => x.AddAsync(It.IsAny<Amenity>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
