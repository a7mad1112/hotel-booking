using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Application.Features.Cities;
using HotelBooking.Infrastructure.Authentication;
using HotelBooking.Infrastructure.ExternalServices.Cloudinary;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Interceptors;
using HotelBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");
        }


        services.AddSingleton<AuditableEntityInterceptor>();


        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString);

            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped(
            typeof(IRepository<>),
            typeof(Repository<>));

        services.Scan(scan => scan
            .FromAssemblyOf<AssemblyReference>()
            .AddClasses(classes =>
                classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes =>
                classes.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );


        services.Configure<JwtOptions>(
            configuration.GetSection(
                JwtOptions.SectionName));
        services.Configure<CloudinaryOptions>(
            configuration.GetSection(CloudinaryOptions.SectionName));

        return services;
    }
}