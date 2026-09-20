using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace HotelBooking.Tests.Integration.Persistence;

public class UserRegistrationRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("hotelbooking_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private ApplicationDbContext _context = null!;
    private UserRegistrationRepository _repository = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        _context = new ApplicationDbContext(options);

        await _context.Database.EnsureCreatedAsync();

        _repository = new UserRegistrationRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_DuplicateEmail_ThrowsDuplicateEmailException()
    {
        // Arrange
        var firstUser = new User
        {
            Email = "ahmad@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer
        };

        await _repository.AddAsync(
            firstUser,
            CancellationToken.None);

        await _repository.SaveChangesAsync(
            CancellationToken.None);

        var secondUser = new User
        {
            Email = "ahmad@example.com",
            PasswordHash = "another-hash",
            Role = UserRole.Customer
        };

        await _repository.AddAsync(
            secondUser,
            CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEmailException>(() => _repository.SaveChangesAsync(
            CancellationToken.None));
    }

    [Fact]
    public async Task SaveChangesAsync_DuplicateEmail_DoesNotCreateSecondUser()
    {
        // Arrange
        var firstUser = new User
        {
            Email = "ahmad@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer
        };

        await _repository.AddAsync(
            firstUser,
            CancellationToken.None);

        await _repository.SaveChangesAsync(
            CancellationToken.None);

        var secondUser = new User
        {
            Email = "ahmad@example.com",
            PasswordHash = "another-hash",
            Role = UserRole.Customer
        };

        await _repository.AddAsync(
            secondUser,
            CancellationToken.None);

        // Act
        await Assert.ThrowsAsync<DuplicateEmailException>(() => _repository.SaveChangesAsync(
            CancellationToken.None));

        // Assert
        await using var verificationContext =
            new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(_postgresContainer.GetConnectionString())
                    .Options);

        var users = await verificationContext.Users
            .Where(x => x.Email == "ahmad@example.com")
            .ToListAsync();

        Assert.Single(users);
    }
}