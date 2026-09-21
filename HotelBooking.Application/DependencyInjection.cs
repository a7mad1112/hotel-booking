using HotelBooking.Application.Features.Authentication.Register;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using HotelBooking.Application.Features.Authentication.Login;
using HotelBooking.Application.Features.Cities.CreateCity;
using HotelBooking.Application.Features.Cities.DeleteCity;
using HotelBooking.Application.Features.Cities.GetCities;
using HotelBooking.Application.Features.Cities.UpdateCity;

namespace HotelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<RegisterService>();
        services.AddScoped<LoginService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddScoped<CreateCityService>();
        services.AddScoped<GetCitiesService>();
        services.AddScoped<DeleteCityService>();
        services.AddScoped<UpdateCityService>();
        return services;
    }
}