using HotelBooking.API.Authorization;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Tests.Unit.Features.Authorization;

public class AuthorizationPolicyTests
{
    private static IAuthorizationPolicyProvider CreatePolicyProvider()
    {
        var services = new ServiceCollection();

        services.AddHotelBookingAuthorization();

        var provider = services
            .BuildServiceProvider()
            .GetRequiredService<IAuthorizationPolicyProvider>();

        return provider;
    }

    [Fact]
    public async Task ManageHotels_AllowsAdminAndOwner()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync(
            AuthorizationPolicies.ManageHotels);
        Assert.NotNull(policy);

        var roleRequirement = Assert.Single(
            policy!.Requirements
                .OfType<RolesAuthorizationRequirement>());

        Assert.Contains(
            nameof(UserRole.Admin),
            roleRequirement.AllowedRoles);

        Assert.Contains(
            nameof(UserRole.Owner),
            roleRequirement.AllowedRoles);
    }

    [Fact]
    public async Task ManageRooms_AllowsAdminAndOwner()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync(
            AuthorizationPolicies.ManageRooms);

        Assert.NotNull(policy);

        var roleRequirement = Assert.Single(
            policy!.Requirements
                .OfType<RolesAuthorizationRequirement>());

        Assert.Contains(
            nameof(UserRole.Admin),
            roleRequirement.AllowedRoles);

        Assert.Contains(
            nameof(UserRole.Owner),
            roleRequirement.AllowedRoles);
    }

    [Fact]
    public async Task ManageCities_RequiresAdmin()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync(
            AuthorizationPolicies.ManageCities);

        Assert.NotNull(policy);

        var roleRequirement = Assert.Single(
            policy!.Requirements
                .OfType<RolesAuthorizationRequirement>());

        Assert.Contains(
            nameof(UserRole.Admin),
            roleRequirement.AllowedRoles);

        Assert.DoesNotContain(
            nameof(UserRole.Owner),
            roleRequirement.AllowedRoles);
    }

    [Fact]
    public async Task CreateBooking_AllowsCustomerOnly()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync(
            AuthorizationPolicies.CreateBooking);

        Assert.NotNull(policy);

        var roleRequirement = Assert.Single(
            policy!.Requirements
                .OfType<RolesAuthorizationRequirement>());

        Assert.Contains(
            nameof(UserRole.Customer),
            roleRequirement.AllowedRoles);

        Assert.DoesNotContain(
            nameof(UserRole.Owner),
            roleRequirement.AllowedRoles);

        Assert.DoesNotContain(
            nameof(UserRole.Admin),
            roleRequirement.AllowedRoles);
    }

    [Fact]
    public async Task ViewHotels_AllowsAllAuthenticatedRoles()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync(
            AuthorizationPolicies.ViewHotels);

        Assert.NotNull(policy);

        var roleRequirement = Assert.Single(
            policy!.Requirements
                .OfType<RolesAuthorizationRequirement>());

        Assert.Contains(
            nameof(UserRole.Customer),
            roleRequirement.AllowedRoles);

        Assert.Contains(
            nameof(UserRole.Owner),
            roleRequirement.AllowedRoles);

        Assert.Contains(
            nameof(UserRole.Admin),
            roleRequirement.AllowedRoles);
    }
}