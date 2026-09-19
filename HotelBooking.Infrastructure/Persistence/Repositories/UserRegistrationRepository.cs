using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public class UserRegistrationRepository : IUserRegistrationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRegistrationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(
            user => user.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}