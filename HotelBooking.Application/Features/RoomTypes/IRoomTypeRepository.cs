using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.RoomTypes;

public interface IRoomTypeRepository : IRepository<RoomType>
{
    Task<bool> HasRoomsAsync(int roomTypeId, CancellationToken cancellationToken);

    Task<(List<Room> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
}