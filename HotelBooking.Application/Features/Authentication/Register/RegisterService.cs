using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Application.Features.Authentication.Register;

public sealed class RegisterService
{
    private readonly IUserRegistrationRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterService(
        IUserRegistrationRepository repository,
        IPasswordHasher<User> passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<User>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Normal business validation / friendly response
        var exists = await _repository.EmailExistsAsync(
            normalizedEmail,
            cancellationToken);

        if (exists)
        {
            return Result<User>.Failure(
                "A user with this email already exists.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            Role = UserRole.Customer
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            password);

        await _repository.AddAsync(
            user,
            cancellationToken);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        // prevents race condition
        catch (DuplicateEmailException)
        {
            return Result<User>.Failure(
                "A user with this email already exists.");
        }

        return Result<User>.Success(user);
    }
}