using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Users.GetBookingHistory;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Users;

public interface IUserRepository : IRepository<User>
{
    Task<List<GetBookingHistoryResponse>> GetBookingHistoryAsync(int userId, int count,
        CancellationToken cancellationToken);
}