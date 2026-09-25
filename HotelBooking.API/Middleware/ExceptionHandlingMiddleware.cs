using System.Security.Claims;
using System.Text.Json;
using HotelBooking.API.Models;

namespace HotelBooking.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogError(
                exception,
                "Unhandled exception while processing {RequestMethod} {RequestPath}. UserId: {UserId}",
                context.Request.Method,
                context.Request.Path,
                userId ?? "anonymous");

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}