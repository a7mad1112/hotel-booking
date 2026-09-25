using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Reviews;

public interface IReviewRepository : IRepository<Review>
{
    Task<(List<Review> Items, int TotalCount)> GetPagedByHotelIdAsync(
        int hotelId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<bool> HasUserBookedHotelAsync(
        int userId,
        int hotelId,
        CancellationToken cancellationToken);
}
