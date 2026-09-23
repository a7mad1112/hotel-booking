using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HotelBooking.API.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddHotelBookingAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.ManageCities,
                policy =>
                {
                    policy.RequireRole(
                        nameof(UserRole.Admin));
                });

            options.AddPolicy(
                AuthorizationPolicies.ManageHotels,
                policy =>
                {
                    policy.RequireRole(
                        nameof(UserRole.Admin),
                        nameof(UserRole.Owner));
                });

            options.AddPolicy(
                AuthorizationPolicies.ManageRooms,
                policy =>
                {
                    policy.RequireRole(
                        nameof(UserRole.Admin),
                        nameof(UserRole.Owner));
                });

            options.AddPolicy(
                AuthorizationPolicies.CreateBooking,
                policy =>
                {
                    policy.RequireRole(
                        nameof(UserRole.Customer));
                });

            options.AddPolicy(
                AuthorizationPolicies.ViewHotels,
                policy =>
                {
                    policy.RequireRole(
                        nameof(UserRole.Customer),
                        nameof(UserRole.Owner),
                        nameof(UserRole.Admin));
                });

            options.AddPolicy(AuthorizationPolicies.ManageRoomTypes,
                policy => { policy.RequireRole(nameof(UserRole.Admin)); });

            options.AddPolicy(AuthorizationPolicies.ManageDeals,
                policy =>
                {
                    policy.RequireRole(
                        nameof(UserRole.Admin),
                        nameof(UserRole.Owner));
                });
        });

        return services;
    }
}