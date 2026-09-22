using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Hotels;

public interface IHotelRepository : IRepository<Hotel>
{
    Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken);

    Task<bool> OwnerExistsAsync(int ownerId, CancellationToken cancellationToken);

    Task<(List<Hotel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Hotel?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken);
}