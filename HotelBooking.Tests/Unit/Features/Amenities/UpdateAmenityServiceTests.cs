using HotelBooking.Application.Features.Amenities;
using HotelBooking.Application.Features.Amenities.UpdateAmenity;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Amenities;

public class UpdateAmenityServiceTests
{
    [Fact]
    public async Task UpdateAsync_ValidAmenity_ReturnsSuccess()
    {
        var repository = new Mock<IAmenityRepository>();
        var existingAmenity = new Amenity { Id = 1, Name = "Wi-Fi", Description = "Old desc" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAmenity);

        repository
            .Setup(x => x.ExistsByNameAsync("High-Speed Wi-Fi", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new UpdateAmenityService(repository.Object);

        var request = new UpdateAmenityRequest
        {
            Name = "High-Speed Wi-Fi",
            Description = "Updated description"
        };

        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("High-Speed Wi-Fi", result.Value!.Name);
        Assert.Equal("Updated description", result.Value.Description);

        repository.Verify(x => x.Update(existingAmenity), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsFailure()
    {
        var repository = new Mock<IAmenityRepository>();
        repository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity?)null);

        var service = new UpdateAmenityService(repository.Object);

        var request = new UpdateAmenityRequest
        {
            Name = "Wi-Fi"
        };

        var result = await service.UpdateAsync(99, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Amenity not found.", result.Error);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateName_ReturnsFailure()
    {
        var repository = new Mock<IAmenityRepository>();
        var existingAmenity = new Amenity { Id = 1, Name = "Wi-Fi" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAmenity);

        repository
            .Setup(x => x.ExistsByNameAsync("Pool", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new UpdateAmenityService(repository.Object);

        var request = new UpdateAmenityRequest
        {
            Name = "Pool"
        };

        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("An amenity with the same name already exists.", result.Error);
        repository.Verify(x => x.Update(It.IsAny<Amenity>()), Times.Never);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
