using System.Security.Claims;
using HotelBooking.API.Extensions;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Tests.Unit.API.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_WithValidClaim_ReturnsParsedInteger()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "42") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetUserId_WithMissingOrInvalidClaim_ReturnsNull()
    {
        // Arrange
        var principalWithoutClaim = new ClaimsPrincipal(new ClaimsIdentity());
        var principalWithInvalidClaim = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "not-an-int") }, "TestAuth"));

        // Act & Assert
        Assert.Null(principalWithoutClaim.GetUserId());
        Assert.Null(principalWithInvalidClaim.GetUserId());
    }

    [Fact]
    public void TryGetUserId_WithValidClaim_ReturnsTrueAndSetsUserId()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "100") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        // Act
        var success = principal.TryGetUserId(out var userId);

        // Assert
        Assert.True(success);
        Assert.Equal(100, userId);
    }

    [Fact]
    public void TryGetUserId_WithMissingClaim_ReturnsFalseAndZero()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var success = principal.TryGetUserId(out var userId);

        // Assert
        Assert.False(success);
        Assert.Equal(0, userId);
    }

    [Fact]
    public void IsAdmin_WithAdminRole_ReturnsTrue()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, nameof(UserRole.Admin)) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        // Act & Assert
        Assert.True(principal.IsAdmin());
    }

    [Fact]
    public void IsAdmin_WithoutAdminRole_ReturnsFalse()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, nameof(UserRole.Customer)) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        // Act & Assert
        Assert.False(principal.IsAdmin());
    }
}
