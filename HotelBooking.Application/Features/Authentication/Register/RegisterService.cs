using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Application.Features.Authentication.Register;

public sealed class RegisterService : IScopedService
{
    private readonly IUserRegistrationRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(
        IUserRegistrationRepository repository,
        IPasswordHasher<User> passwordHasher,
        ILogger<RegisterService> logger)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<ResultOfT<User>> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var exists = await _repository.EmailExistsAsync(normalizedEmail, cancellationToken);

        if (exists)
        {
            _logger.LogWarning("Registration attempt rejected because the email already exists.");

            return ResultOfT<User>.Failure("A user with this email already exists.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            Role = UserRole.Customer
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _repository.AddAsync(user, cancellationToken);

        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateEmailException)
        {
            _logger.LogWarning("Registration failed because a duplicate email was detected during persistence.");

            return ResultOfT<User>.Failure("A user with this email already exists.");
        }

        _logger.LogInformation("User registered successfully. UserId {UserId}, Role {Role}", user.Id, user.Role);

        return ResultOfT<User>.Success(user);
    }
}