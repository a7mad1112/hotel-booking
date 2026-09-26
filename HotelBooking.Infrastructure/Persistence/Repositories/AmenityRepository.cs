using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Amenities;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class AmenityRepository : Repository<Amenity>, IAmenityRepository, IScopedService
{
    public AmenityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Amenity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = DbContext.Amenities
            .AsNoTracking();

        return await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToPagedListAsync(page, pageSize, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        var query = DbContext.Amenities
            .AsNoTracking()
            .Where(x => x.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasHotelsAsync(int amenityId, CancellationToken cancellationToken)
    {
        return await DbContext.HotelAmenities
            .AnyAsync(x => x.AmenityId == amenityId, cancellationToken);
    }
}
