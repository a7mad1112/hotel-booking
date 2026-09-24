using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotelBooking.Tests.Unit.Features.Authentication.Register;

public class RegisterServiceTests
{
    private readonly FakeUserRegistrationRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly RegisterService _service;

    public RegisterServiceTests()
    {
        _repository = new FakeUserRegistrationRepository();
        _passwordHasher = new PasswordHasher<User>();

        _service = new RegisterService(
            _repository,
            _passwordHasher,
            NullLogger<RegisterService>.Instance);
    }

    [Fact]
    public async Task RegisterAsync_ValidData_CreatesCustomer()
    {
        // Arrange
        const string email = "ahmed@example.com";
        const string password = "Password123!";

        // Act
        var result = await _service.RegisterAsync(
            email,
            password,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(email, result.Value!.Email);
        Assert.Equal(UserRole.Customer, result.Value.Role);

        Assert.NotEqual(string.Empty, result.Value.PasswordHash);
        Assert.Single(_repository.Users);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        const string email = "ahmed@example.com";

        _repository.Users.Add(new User
        {
            Email = email,
            Role = UserRole.Customer,
            PasswordHash = "password-hash"
        });

        // Act
        var result = await _service.RegisterAsync(
            email,
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "A user with this email already exists.",
            result.Error);

        Assert.Single(_repository.Users);
    }

    [Fact]
    public async Task RegisterAsync_ValidPassword_StoresHashedPassword()
    {
        // Arrange
        const string email = "ahmed@example.com";
        const string password = "Password123!";

        // Act
        var result = await _service.RegisterAsync(
            email,
            password,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var user = result.Value!;

        Assert.NotEqual(password, user.PasswordHash);

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        Assert.Equal(
            PasswordVerificationResult.Success,
            verificationResult);
    }

    [Fact]
    public async Task RegisterAsync_EmailWithWhitespaceAndUppercase_NormalizesEmail()
    {
        // Arrange
        const string email = "  Ahmed@Example.COM  ";

        // Act
        var result = await _service.RegisterAsync(
            email,
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(
            "ahmed@example.com",
            result.Value!.Email);
    }

    [Fact]
    public async Task RegisterAsync_AlwaysAssignsCustomerRole()
    {
        // Arrange
        const string email = "attacker@example.com";

        // Act
        var result = await _service.RegisterAsync(
            email,
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(
            UserRole.Customer,
            result.Value!.Role);
    }

    private sealed class FakeUserRegistrationRepository
        : IUserRegistrationRepository
    {
        public List<User> Users { get; } = [];

        public Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken)
        {
            var exists = Users.Any(user => user.Email == email);
            return Task.FromResult(exists);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}