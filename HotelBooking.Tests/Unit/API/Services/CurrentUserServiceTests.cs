using System.Security.Claims;
using HotelBooking.API.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace HotelBooking.Tests.Unit.API.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

    private CurrentUserService CreateService(ClaimsPrincipal? user)
    {
        var context = new DefaultHttpContext();
        if (user is not null)
        {
            context.User = user;
        }

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(user is null ? null : context);
        return new CurrentUserService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void UserId_WhenUserHasValidNameIdentifier_ReturnsParsedUserId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "42")
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        var service = CreateService(principal);

        // Act & Assert
        Assert.Equal(42, service.UserId);
        Assert.Equal(42, service.GetRequiredUserId());
        Assert.True(service.IsAuthenticated);
    }

    [Fact]
    public void UserId_WhenUserIsAnonymousOrNull_ReturnsNullAndThrowsOnRequired()
    {
        // Arrange
        var service = CreateService(null);

        // Act & Assert
        Assert.Null(service.UserId);
        Assert.False(service.IsAuthenticated);
        Assert.False(service.IsAdmin);
        Assert.Throws<UnauthorizedAccessException>(() => service.GetRequiredUserId());
    }

    [Fact]
    public void IsAdmin_WhenUserHasAdminRole_ReturnsTrue()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        var service = CreateService(principal);

        // Act & Assert
        Assert.True(service.IsAdmin);
    }

    [Fact]
    public void IsAdmin_WhenUserHasNonAdminRole_ReturnsFalse()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "2"),
            new Claim(ClaimTypes.Role, "Customer")
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        var service = CreateService(principal);

        // Act & Assert
        Assert.False(service.IsAdmin);
    }
}
