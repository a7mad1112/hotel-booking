using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class RoomRepository : Repository<Room>, IRoomRepository, IScopedService
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Room> Items, int TotalCount)> GetPagedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Rooms
            .AsNoTracking()
            .Include(x => x.RoomType)
            .Include(x => x.Hotel);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.RoomNumber)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Room?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext.Rooms
            .AsNoTracking()
            .Include(x => x.RoomType)
            .Include(x => x.Hotel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> HotelExistsAsync(int hotelId, CancellationToken cancellationToken)
    {
        return await DbContext.Hotels
            .AnyAsync(x => x.Id == hotelId, cancellationToken);
    }

    public async Task<bool> RoomTypeExistsAsync(int roomTypeId, CancellationToken cancellationToken)
    {
        return await DbContext.RoomTypes.AnyAsync(x => x.Id == roomTypeId, cancellationToken);
    }

    public async Task<bool> HasDependenciesAsync(int roomId, CancellationToken cancellationToken)
    {
        return await DbContext.Bookings
            .AnyAsync(x => x.RoomId == roomId, cancellationToken);
    }

    public async Task<bool> HasBookingsAsync(int roomId, CancellationToken cancellationToken)
    {
        return await DbContext.Bookings.AnyAsync(x => x.RoomId == roomId, cancellationToken);
    }
}