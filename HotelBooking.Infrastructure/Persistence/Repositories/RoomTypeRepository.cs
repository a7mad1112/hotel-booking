using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.RoomTypes;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository, IScopedService
{
    public RoomTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasRoomsAsync(int roomTypeId, CancellationToken cancellationToken)
    {
        return await DbContext.Rooms.AnyAsync(x => x.RoomTypeId == roomTypeId, cancellationToken);
    }

    public async Task<(List<Room> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Rooms
            .AsNoTracking()
            .Include(x => x.Hotel)
            .Include(x => x.RoomType);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.OrderBy(x => x.RoomNumber)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}