using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Extensions;

public static class ValidationExtensions
{
    public static ActionResult ValidationProblem(
        this ControllerBase controller,
        ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            controller.ModelState.AddModelError(
                error.PropertyName,
                error.ErrorMessage);
        }

        return controller.ValidationProblem(
            controller.ModelState);
    }
}