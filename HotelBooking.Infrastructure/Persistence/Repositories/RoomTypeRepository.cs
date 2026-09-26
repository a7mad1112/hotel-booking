using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.RoomTypes;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository, IScopedService
{
    public RoomTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<RoomType> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.RoomTypes
            .AsNoTracking();

        return await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToPagedListAsync(page, pageSize, cancellationToken);
    }

    public async Task<bool> HasRoomsAsync(int roomTypeId, CancellationToken cancellationToken)
    {
        return await DbContext.Rooms
            .AnyAsync(x => x.RoomTypeId == roomTypeId, cancellationToken);
    }
}