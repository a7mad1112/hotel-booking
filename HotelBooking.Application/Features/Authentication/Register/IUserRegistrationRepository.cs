using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Authentication.Register;

public interface IUserRegistrationRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}