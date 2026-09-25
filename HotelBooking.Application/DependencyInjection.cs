using FluentValidation;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.Scan(scan => scan
            .FromAssemblies(AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo<ITransientService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

        return services;
    }
}