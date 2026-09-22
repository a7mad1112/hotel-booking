using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class HotelRepository
    : Repository<Hotel>, IHotelRepository, IScopedService
{
    public HotelRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }


    public async Task<Hotel?> GetDetailsByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await DbContext.Hotels
            .Include(x => x.City)
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }


    public async Task<List<Hotel>> GetDetailsAsync(
        CancellationToken cancellationToken)
    {
        return await DbContext.Hotels
            .Include(x => x.City)
            .Include(x => x.Owner)
            .ToListAsync(cancellationToken);
    }


    public async Task<bool> CityExistsAsync(
        int cityId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Cities
            .AnyAsync(
                x => x.Id == cityId,
                cancellationToken);
    }


    public async Task<bool> OwnerExistsAsync(
        int ownerId,
        CancellationToken cancellationToken)
    {
        return await DbContext.Users
            .AnyAsync(
                x => x.Id == ownerId,
                cancellationToken);
    }
}