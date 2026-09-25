using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelBooking.API.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<
            HotelBooking.Application.AssemblyReference>();

        services.AddValidatorsFromAssemblyContaining<
            HotelBooking.API.AssemblyReference>();

        services.AddScoped<FluentValidationFilter>();

        return services;
    }
}

public sealed class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType =
                typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (_serviceProvider.GetService(validatorType)
                is not IValidator validator)
            {
                continue;
            }

            var validationContext =
                new ValidationContext<object>(argument);

            var result =
                await validator.ValidateAsync(
                    validationContext,
                    context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(error => error.ErrorMessage).ToArray());

                context.Result = new BadRequestObjectResult(new
                {
                    message = "Validation failed.",
                    errors
                });

                return;
            }
        }

        await next();
    }
}