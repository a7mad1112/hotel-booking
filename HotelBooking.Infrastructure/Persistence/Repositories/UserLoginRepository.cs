using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class UserLoginRepository : IUserLoginRepository, IScopedService
{
    private readonly ApplicationDbContext _dbContext;

    public UserLoginRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Email == email,
                cancellationToken);
    }
}