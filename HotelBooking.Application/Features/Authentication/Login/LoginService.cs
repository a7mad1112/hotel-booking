using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Application.Features.Authentication.Login;

public sealed class LoginService: IScopedService
{
    private readonly IUserLoginRepository _repository;
    private readonly IJwtTokenGenerator _generator;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginService(
        IUserLoginRepository repository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenGenerator generator)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _generator = generator;
    }

    public async Task<ResultOfT<LoginResponse>> LoginAsync(string email, string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _repository.GetUserByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return ResultOfT<LoginResponse>.Failure(
                "Invalid email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return ResultOfT<LoginResponse>.Failure("Invalid email or password.");
        }

        var token = _generator.Generate(user);

        return ResultOfT<LoginResponse>.Success(
            new LoginResponse
            {
                AccessToken = token.AccessToken,
                ExpiresAt = token.ExpiresAt,
            }
        );
    }
}