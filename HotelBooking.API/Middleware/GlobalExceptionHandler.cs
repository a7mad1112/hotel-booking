using System.Security.Claims;
using HotelBooking.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

        _logger.LogError(
            exception,
            "Unhandled exception occurred while processing {RequestMethod} {RequestPath}. UserId: {UserId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            userId);

        var (statusCode, title, detail) = exception switch
        {
            DuplicateEmailException or DuplicateRoomNumberException or OverlappingBookingException =>
                (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Not Found", exception.Message),
            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            ArgumentException =>
                (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
            _ =>
                (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        var handled = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        if (!handled)
        {
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, options: null, contentType: "application/problem+json", cancellationToken: cancellationToken);
        }

        return true;
    }
}
