using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public class UserRegistrationRepository : IUserRegistrationRepository, IScopedService
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

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (IsDuplicateEmailViolation(ex))
        {
            throw new DuplicateEmailException();
        }
    }

    private static bool IsDuplicateEmailViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
               && postgresException.ConstraintName == "IX_users_Email";
    }
}