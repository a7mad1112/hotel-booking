using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using HotelBooking.Application.Features.Authentication.Login;

namespace HotelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<RegisterService>();
        services.AddScoped<LoginService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}