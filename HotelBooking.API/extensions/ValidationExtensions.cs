using FluentValidation;
using FluentValidation.AspNetCore;

namespace HotelBooking.API.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(
        this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<
            HotelBooking.Application.AssemblyReference>();

        services.AddValidatorsFromAssemblyContaining<
            HotelBooking.API.AssemblyReference>();

        return services;
    }
}