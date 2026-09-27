using HotelBooking.Application.Features.Hotels;
using HotelBooking.Application.Features.Hotels.DeleteHotel;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Hotels.DeleteHotel;

public class DeleteHotelServiceTests
{
    [Fact]
    public async Task DeleteAsync_AdminCanDeleteAnyHotel()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            Name = "Grand Hotel"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        var service = new DeleteHotelService(
            repository.Object);

        var result =
            await service.DeleteAsync(
                1,
                currentUserId: 99,
                isAdmin: true,
                CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.Delete(hotel),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_OwnerCanDeleteOwnHotel()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            Name = "Grand Hotel"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        var service = new DeleteHotelService(
            repository.Object);

        var result =
            await service.DeleteAsync(
                1,
                currentUserId: 10,
                isAdmin: false,
                CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.Delete(hotel),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DifferentOwner_ReturnsFailure()
    {
        var repository = new Mock<IHotelRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            Name = "Grand Hotel"
        };

        repository
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        var service = new DeleteHotelService(
            repository.Object);

        var result =
            await service.DeleteAsync(
                1,
                currentUserId: 99,
                isAdmin: false,
                CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "You are not allowed to delete this hotel.",
            result.Error);

        repository.Verify(
            x => x.Delete(
                It.IsAny<Hotel>()),
            Times.Never);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_HotelWithImages_EnqueuesImagePublicIdsToOutbox()
    {
        var repository = new Mock<IHotelRepository>();
        var outboxRepository = new Mock<HotelBooking.Application.Common.Images.IImageDeletionOutboxRepository>();

        var hotel = new Hotel
        {
            Id = 1,
            OwnerId = 10,
            Name = "Grand Hotel"
        };

        var publicIds = new List<string> { "hotels/hotel_1", "hotels/hotel_2" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        repository
            .Setup(x => x.HasDependenciesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repository
            .Setup(x => x.GetImagePublicIdsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publicIds);

        var service = new DeleteHotelService(
            repository.Object,
            currentUserService: null,
            outboxRepository: outboxRepository.Object);

        var result = await service.DeleteAsync(
            1,
            currentUserId: 10,
            isAdmin: false,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        outboxRepository.Verify(
            x => x.EnqueueRangeAsync(publicIds, It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.Delete(hotel),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}