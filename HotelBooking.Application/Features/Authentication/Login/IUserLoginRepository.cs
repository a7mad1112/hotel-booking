using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Authentication.Login;

public interface IUserLoginRepository
{
    Task<User?> GetUserByEmailAsync(string email
        , CancellationToken cancellationToken);
}