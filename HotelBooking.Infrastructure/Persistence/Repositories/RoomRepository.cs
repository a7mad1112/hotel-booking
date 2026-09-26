using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class RoomRepository : Repository<Room>, IRoomRepository, IScopedService
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Room> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<Room> query = DbContext.Rooms
            .AsNoTracking()
            .Include(x => x.RoomType)
            .Include(x => x.Hotel);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmed = search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.RoomNumber, $"%{trimmed}%") ||
                                     EF.Functions.ILike(x.Hotel.Name, $"%{trimmed}%"));
        }

        return await query
            .OrderBy(x => x.RoomNumber)
            .ThenBy(x => x.Id)
            .ToPagedListAsync(page, pageSize, cancellationToken);
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

    public async Task<Room?> GetForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext.Rooms
            .Include(x => x.Hotel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private static bool IsDuplicateRoomNumberViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
               && postgresException.ConstraintName ==
               "IX_rooms_HotelId_RoomNumber";
    }

    public async Task<bool> RoomNumberExistsAsync(int hotelId, string roomNumber, CancellationToken cancellationToken)
    {
        return await DbContext.Rooms
            .AnyAsync(x => x.HotelId == hotelId && x.RoomNumber == roomNumber, cancellationToken);
    }
}