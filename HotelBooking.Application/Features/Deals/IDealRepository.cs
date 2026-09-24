using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Deals;

public interface IDealRepository : IRepository<Deal>
{
    Task<(List<Deal> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<Deal?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken);

    Task<Hotel?> GetHotelAsync(int hotelId, CancellationToken cancellationToken);

    Task<bool> HasOverlappingDealAsync(int hotelId, DateTime startDate, DateTime endDate, int? excludedDealId,
        CancellationToken cancellationToken);

    Task<List<Deal>> GetFeaturedAsync(DateTime now, int limit, CancellationToken cancellationToken);

    Task<Deal?> GetApplicableDealAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate,
        CancellationToken cancellationToken);
}