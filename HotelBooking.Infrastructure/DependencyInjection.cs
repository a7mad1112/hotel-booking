using HotelBooking.Application.Common.Email;
using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Application.Features.Cities;
using HotelBooking.Infrastructure.Authentication;
using HotelBooking.Infrastructure.ExternalServices.Cloudinary;
using HotelBooking.Infrastructure.ExternalServices.Email;
using HotelBooking.Infrastructure.ExternalServices.Invoicing;
using HotelBooking.Infrastructure.ExternalServices.Stripe;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Interceptors;
using HotelBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.Scan(scan => scan
            .FromAssemblies(AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo<ITransientService>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));

        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

        return services;
    }
}