using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Authentication.Login;

public class LoginServiceTests
{
    private readonly Mock<IUserLoginRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;

    private readonly LoginService _service;

    public LoginServiceTests()
    {
        _repositoryMock = new Mock<IUserLoginRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _service = new LoginService(
            _repositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var user = CreateUser();

        _repositoryMock
            .Setup(x => x.GetUserByEmailAsync(
                "ahmed@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyHashedPassword(
                user,
                user.PasswordHash,
                "Password123!"))
            .Returns(PasswordVerificationResult.Success);

        var token = new JwtTokenResult
        {
            AccessToken = "test-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        _jwtTokenGeneratorMock
            .Setup(x => x.Generate(user))
            .Returns(token);

        // Act
        var result = await _service.LoginAsync(
            "ahmed@example.com",
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            "test-token",
            result.Value!.AccessToken);

        Assert.Equal(
            token.ExpiresAt,
            result.Value.ExpiresAt);

        _jwtTokenGeneratorMock.Verify(
            x => x.Generate(user),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsFailure()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetUserByEmailAsync(
                "unknown@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.LoginAsync(
            "unknown@example.com",
            "Password123!",
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Invalid email or password.",
            result.Error);

        _passwordHasherMock.Verify(
            x => x.VerifyHashedPassword(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _jwtTokenGeneratorMock.Verify(
            x => x.Generate(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        // Arrange
        var user = CreateUser();

        _repositoryMock
            .Setup(x => x.GetUserByEmailAsync(
                "ahmed@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyHashedPassword(
                user,
                user.PasswordHash,
                "WrongPassword"))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _service.LoginAsync(
            "ahmed@example.com",
            "WrongPassword",
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Invalid email or password.",
            result.Error);

        _jwtTokenGeneratorMock.Verify(
            x => x.Generate(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_EmailIsNormalized_BeforeRepositoryLookup()
    {
        // Arrange
        var user = CreateUser();

        _repositoryMock
            .Setup(x => x.GetUserByEmailAsync(
                "ahmed@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyHashedPassword(
                user,
                user.PasswordHash,
                "Password123!"))
            .Returns(PasswordVerificationResult.Success);

        _jwtTokenGeneratorMock
            .Setup(x => x.Generate(user))
            .Returns(new JwtTokenResult
            {
                AccessToken = "test-token",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
            });

        // Act
        await _service.LoginAsync(
            "  ahmed@Example.COM  ",
            "Password123!",
            CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.GetUserByEmailAsync(
                "ahmed@example.com",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = 1,
            Email = "ahmed@example.com",
            PasswordHash = "hashed-password",
            Role = UserRole.Customer
        };
    }
}