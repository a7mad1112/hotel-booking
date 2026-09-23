using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken);

    Task<(List<Room> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<bool> HotelExistsAsync(int hotelId, CancellationToken cancellationToken);

    Task<bool> RoomTypeExistsAsync(int roomTypeId, CancellationToken cancellationToken);

    Task<bool> HasDependenciesAsync(int roomId, CancellationToken cancellationToken);

    Task<bool> HasBookingsAsync(int roomId, CancellationToken cancellationToken);
}