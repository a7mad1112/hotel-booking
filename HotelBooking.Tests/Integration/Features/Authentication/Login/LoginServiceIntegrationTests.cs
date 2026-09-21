using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Authentication;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace HotelBooking.Tests.Integration.Features.Authentication.Login;

public class LoginServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("hotelbooking_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private ApplicationDbContext _context = null!;
    private LoginService _loginService = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        _context = new ApplicationDbContext(options);

        await _context.Database.EnsureCreatedAsync();

        var repository = new UserLoginRepository(_context);

        var passwordHasher = new PasswordHasher<User>();

        var jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey =
                "this-is-a-development-test-secret-key-that-is-long-enough",
            Issuer = "HotelBooking.API",
            Audience = "HotelBooking.Client",
            ExpirationMinutes = 60
        });

        var jwtTokenGenerator =
            new JwtTokenGenerator(jwtOptions);

        _loginService = new LoginService(
            repository,
            passwordHasher,
            jwtTokenGenerator);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    private async Task<User> CreateUserAsync(
        string email = "ahmed@example.com",
        string password = "Password123!")
    {
        var user = new User
        {
            Id = 1,
            Email = email,
            Role = UserRole.Customer
        };

        var passwordHasher = new PasswordHasher<User>();

        user.PasswordHash =
            passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithJwt()
    {
        // Arrange
        await CreateUserAsync();

        // Act
        var result = await _loginService.LoginAsync(
            "ahmed@example.com",
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(
            string.IsNullOrWhiteSpace(
                result.Value!.AccessToken));

        Assert.True(
            result.Value.ExpiresAt > DateTimeOffset.UtcNow);

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(
            result.Value.AccessToken);

        Assert.Equal(
            "HotelBooking.API",
            token.Issuer);

        Assert.Contains(
            token.Audiences,
            audience => audience == "HotelBooking.Client");

        Assert.Contains(
            token.Claims,
            claim =>
                claim.Type == ClaimTypes.Role &&
                claim.Value == nameof(UserRole.Customer));
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        // Arrange
        await CreateUserAsync();

        // Act
        var result = await _loginService.LoginAsync(
            "ahmed@example.com",
            "WrongPassword!",
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Invalid email or password.",
            result.Error);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsFailure()
    {
        // Act
        var result = await _loginService.LoginAsync(
            "unknown@example.com",
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Invalid email or password.",
            result.Error);
    }
}