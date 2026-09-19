using HotelBooking.Application.Features.Authentication.Register;

namespace HotelBooking.Tests.Unit.Features.Authentication.Register;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_IsValid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "ahmed@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidEmail_IsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "not-email",
            Password = "Password123!"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(request.Email));
    }

    [Fact]
    public async Task Validate_EmptyEmail_IsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = string.Empty,
            Password = "Password123!"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_ShortPassword_IsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "ahmed@example.com",
            Password = "pass12"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(request.Password));
    }

    [Fact]
    public async Task Validate_EmptyPassword_IsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "ahmed@example.com",
            Password = string.Empty
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }
}