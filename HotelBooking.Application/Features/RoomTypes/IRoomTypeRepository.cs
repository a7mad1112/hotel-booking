using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.RoomTypes;

public interface IRoomTypeRepository : IRepository<RoomType>
{
    Task<(List<RoomType> Items, int TotalCount)> GetPagedAsync(int page, int pageSize,
        CancellationToken cancellationToken);

    Task<bool> HasRoomsAsync(int roomTypeId, CancellationToken cancellationToken);
}