using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Users;
using HotelBooking.Application.Features.Users.GetUsers;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Users.GetUsers;

public class GetUsersServiceTests
{
    [Fact]
    public async Task GetUsersAsync_ReturnsPagedUsers()
    {
        var repository = new Mock<IUserRepository>();
        var users = new List<User>
        {
            new() { Id = 1, Email = "admin@example.com", Role = UserRole.Admin, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Email = "owner@example.com", Role = UserRole.Owner, CreatedAt = DateTimeOffset.UtcNow }
        };

        repository
            .Setup(x => x.GetPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 2));

        var service = new GetUsersService(repository.Object);

        var request = new PaginationRequest { Page = 1, PageSize = 10 };
        var result = await service.GetUsersAsync(request, null, null, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("admin@example.com", result.Items[0].Email);
        Assert.Equal(UserRole.Admin, result.Items[0].Role);
        Assert.Equal("owner@example.com", result.Items[1].Email);
        Assert.Equal(UserRole.Owner, result.Items[1].Role);
    }

    [Fact]
    public async Task GetUsersAsync_WithRoleFilter_ReturnsFilteredUsers()
    {
        var repository = new Mock<IUserRepository>();
        var users = new List<User>
        {
            new() { Id = 2, Email = "owner@example.com", Role = UserRole.Owner, CreatedAt = DateTimeOffset.UtcNow }
        };

        repository
            .Setup(x => x.GetPagedAsync(1, 10, "owner", UserRole.Owner, It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 1));

        var service = new GetUsersService(repository.Object);

        var request = new PaginationRequest { Page = 1, PageSize = 10 };
        var result = await service.GetUsersAsync(request, "owner", UserRole.Owner, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(UserRole.Owner, result.Items[0].Role);
    }
}
