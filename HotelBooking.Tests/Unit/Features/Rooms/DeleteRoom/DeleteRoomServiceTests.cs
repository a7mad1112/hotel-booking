using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Application.Features.Rooms.DeleteRoom;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Rooms.DeleteRoom;

public class DeleteRoomServiceTests
{
    [Fact]
    public async Task DeleteAsync_RoomWithImages_EnqueuesImagePublicIdsToOutbox()
    {
        var repository = new Mock<IRoomRepository>();
        var outboxRepository = new Mock<IImageDeletionOutboxRepository>();

        var room = new Room
        {
            Id = 1,
            HotelId = 5,
            RoomNumber = "101",
            Hotel = new Hotel
            {
                Id = 5,
                OwnerId = 10,
                Name = "Grand Hotel"
            }
        };

        var publicIds = new List<string> { "rooms/room_101_1", "rooms/room_101_2" };

        repository
            .Setup(x => x.GetDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        repository
            .Setup(x => x.HasBookingsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repository
            .Setup(x => x.GetImagePublicIdsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publicIds);

        var service = new DeleteRoomService(
            repository.Object,
            outboxRepository.Object);

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
            x => x.Delete(room),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRoomHasBookings_ReturnsFailureAndDoesNotEnqueue()
    {
        var repository = new Mock<IRoomRepository>();
        var outboxRepository = new Mock<IImageDeletionOutboxRepository>();

        var room = new Room
        {
            Id = 1,
            HotelId = 5,
            RoomNumber = "101",
            Hotel = new Hotel
            {
                Id = 5,
                OwnerId = 10,
                Name = "Grand Hotel"
            }
        };

        repository
            .Setup(x => x.GetDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        repository
            .Setup(x => x.HasBookingsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new DeleteRoomService(
            repository.Object,
            outboxRepository.Object);

        var result = await service.DeleteAsync(
            1,
            currentUserId: 10,
            isAdmin: false,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete a room that has related bookings.", result.Error);

        outboxRepository.Verify(
            x => x.EnqueueRangeAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            x => x.Delete(It.IsAny<Room>()),
            Times.Never);
    }
}
