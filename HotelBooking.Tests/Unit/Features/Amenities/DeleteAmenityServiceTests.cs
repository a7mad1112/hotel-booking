using HotelBooking.Application.Features.Amenities;
using HotelBooking.Application.Features.Amenities.DeleteAmenity;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Amenities;

public class DeleteAmenityServiceTests
{
    [Fact]
    public async Task DeleteAsync_ValidAmenity_ReturnsSuccess()
    {
        var repository = new Mock<IAmenityRepository>();
        var existingAmenity = new Amenity { Id = 1, Name = "Spa" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAmenity);

        repository
            .Setup(x => x.HasHotelsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new DeleteAmenityService(repository.Object);

        var result = await service.DeleteAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(x => x.Delete(existingAmenity), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFailure()
    {
        var repository = new Mock<IAmenityRepository>();
        repository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity?)null);

        var service = new DeleteAmenityService(repository.Object);

        var result = await service.DeleteAsync(99, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Amenity not found.", result.Error);
        repository.Verify(x => x.Delete(It.IsAny<Amenity>()), Times.Never);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_AssignedToHotels_ReturnsFailure()
    {
        var repository = new Mock<IAmenityRepository>();
        var existingAmenity = new Amenity { Id = 1, Name = "Pool" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAmenity);

        repository
            .Setup(x => x.HasHotelsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new DeleteAmenityService(repository.Object);

        var result = await service.DeleteAsync(1, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete an amenity that is assigned to hotels.", result.Error);
        repository.Verify(x => x.Delete(It.IsAny<Amenity>()), Times.Never);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
