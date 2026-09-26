using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class HotelRepository
    : Repository<Hotel>, IHotelRepository, IScopedService
{
    public HotelRepository(ApplicationDbContext context) : base(context)
    {
    }


    public async Task<(List<Hotel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<Hotel> query = DbContext.Hotels
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.Owner)
            .Include(x => x.Rooms);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmed = search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{trimmed}%") ||
                                     EF.Functions.ILike(x.City.Name, $"%{trimmed}%") ||
                                     EF.Functions.ILike(x.Location, $"%{trimmed}%"));
        }

        return await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.City.Name)
            .ThenBy(x => x.Id)
            .ToPagedListAsync(page, pageSize, cancellationToken);
    }


    public async Task<Hotel?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext.Hotels
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.Owner)
            .Include(x => x.Images)
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomType)
            .Include(x => x.Rooms)
            .ThenInclude(x => x.Images)
            .Include(x => x.HotelAmenities)
            .ThenInclude(x => x.Amenity)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken)
    {
        return await DbContext.Cities.AnyAsync(x => x.Id == cityId, cancellationToken);
    }


    public async Task<bool> OwnerExistsAsync(int ownerId, CancellationToken cancellationToken)
    {
        return await DbContext.Users
            .AnyAsync(x => x.Id == ownerId && x.Role == UserRole.Owner,
                cancellationToken);
    }

    public async Task<bool> HasDependenciesAsync(int hotelId, CancellationToken cancellationToken)
    {
        return await DbContext.Rooms.AnyAsync(
                   x => x.HotelId == hotelId, cancellationToken) ||
               await DbContext.Reviews.AnyAsync(x => x.HotelId == hotelId, cancellationToken);
    }
}