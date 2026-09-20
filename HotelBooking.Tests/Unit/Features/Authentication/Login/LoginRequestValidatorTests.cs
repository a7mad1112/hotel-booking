using HotelBooking.Application.Features.Authentication.Login;

namespace HotelBooking.Tests.Unit.Features.Authentication.Login;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_IsValid()
    {
        var request = new LoginRequest
        {
            Email = "ahmad@example.com",
            Password = "Password123!"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyEmail_IsInvalid()
    {
        var request = new LoginRequest
        {
            Email = string.Empty,
            Password = "Password123!"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidEmail_IsInvalid()
    {
        var request = new LoginRequest
        {
            Email = "invalid-email",
            Password = "Password123!"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyPassword_IsInvalid()
    {
        var request = new LoginRequest
        {
            Email = "ahmad@example.com",
            Password = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}