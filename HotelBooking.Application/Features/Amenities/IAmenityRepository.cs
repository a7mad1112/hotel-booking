using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Amenities;

public interface IAmenityRepository : IRepository<Amenity>
{
    Task<(List<Amenity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken cancellationToken);

    Task<bool> HasHotelsAsync(int amenityId, CancellationToken cancellationToken);
}
