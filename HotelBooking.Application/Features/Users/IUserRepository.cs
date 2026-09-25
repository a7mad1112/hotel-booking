using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Users.GetBookingHistory;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Users;

public interface IUserRepository : IRepository<User>
{
    Task<List<GetBookingHistoryResponse>> GetBookingHistoryAsync(int userId, int count,
        CancellationToken cancellationToken);

    Task<(List<User> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        UserRole? role,
        CancellationToken cancellationToken);
}