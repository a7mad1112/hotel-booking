using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Application.Features.Rooms.GetRooms;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Rooms.GetRooms;

public class GetRoomsServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsPagedRooms()
    {
        // Arrange
        var repository = new Mock<IRoomRepository>();

        repository
            .Setup(x => x.GetPagedAsync(
                1,
                10,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new List<Room>
                    {
                        new()
                        {
                            Id = 1,
                            HotelId = 10,
                            Hotel = new Hotel { Id = 10, Name = "Grand Hotel" },
                            RoomNumber = "101",
                            RoomTypeId = 2,
                            RoomType = new RoomType { Id = 2, Name = "Deluxe Suite" },
                            PricePerNight = 150m,
                            AdultsCapacity = 2,
                            ChildrenCapacity = 1,
                            Availability = true
                        }
                    },
                    1
                ));

        var service = new GetRoomsService(repository.Object);

        var request = new PaginationRequest
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await service.GetAllAsync(request, null, CancellationToken.None);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);

        var room = result.Items[0];
        Assert.Equal(1, room.Id);
        Assert.Equal(10, room.HotelId);
        Assert.Equal("Grand Hotel", room.HotelName);
        Assert.Equal("101", room.RoomNumber);
        Assert.Equal(2, room.RoomTypeId);
        Assert.Equal("Deluxe Suite", room.RoomTypeName);
        Assert.Equal(150m, room.PricePerNight);
        Assert.Equal(2, room.AdultsCapacity);
        Assert.Equal(1, room.ChildrenCapacity);
        Assert.True(room.Availability);
    }

    [Fact]
    public async Task GetAllAsync_WithSearchTerm_PassesSearchToRepository()
    {
        // Arrange
        var repository = new Mock<IRoomRepository>();

        repository
            .Setup(x => x.GetPagedAsync(
                1,
                10,
                "101",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Room>(), 0));

        var service = new GetRoomsService(repository.Object);

        var request = new PaginationRequest
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await service.GetAllAsync(request, "101", CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        repository.Verify(x => x.GetPagedAsync(1, 10, "101", It.IsAny<CancellationToken>()), Times.Once);
    }
}
