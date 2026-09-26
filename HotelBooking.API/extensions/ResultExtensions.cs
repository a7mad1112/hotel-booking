using HotelBooking.Application.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return ToErrorResult(result.ErrorDetail, result.Error);
    }

    public static ActionResult ToActionResult(this Result result, Func<ActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        return ToErrorResult(result.ErrorDetail, result.Error);
    }

    public static ActionResult<T> ToActionResult<T>(this ResultOfT<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return ToErrorResult(result.ErrorDetail, result.Error);
    }

    public static ActionResult<T> ToActionResult<T>(this ResultOfT<T> result, Func<T, ActionResult<T>> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return ToErrorResult(result.ErrorDetail, result.Error);
    }

    public static ActionResult ToErrorResult(this Result result)
    {
        return ToErrorResult(result.ErrorDetail, result.Error);
    }

    public static ActionResult ToErrorResult<T>(this ResultOfT<T> result)
    {
        return ToErrorResult(result.ErrorDetail, result.Error);
    }

    public static ActionResult ToErrorResult(Error? error, string? fallbackMessage)
    {
        var message = !string.IsNullOrEmpty(error?.Description) ? error.Description : fallbackMessage ?? "An error occurred.";
        var type = error?.Type ?? ErrorType.Failure;
        var responseObj = new { message };

        return type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(responseObj),
            ErrorType.Conflict => new ConflictObjectResult(responseObj),
            ErrorType.Forbidden => new ObjectResult(responseObj) { StatusCode = StatusCodes.Status403Forbidden },
            ErrorType.Unauthorized => new UnauthorizedObjectResult(responseObj),
            ErrorType.Validation => new BadRequestObjectResult(responseObj),
            _ => new BadRequestObjectResult(responseObj)
        };
    }
}
