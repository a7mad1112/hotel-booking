using System.IO;
using System.Text.Json;
using HotelBooking.API.Middleware;
using HotelBooking.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HotelBooking.Tests.Unit.API.Middleware;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock = new();
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock = new();

    private GlobalExceptionHandler CreateHandler() =>
        new(_loggerMock.Object, _problemDetailsServiceMock.Object);

    [Fact]
    public async Task TryHandleAsync_DuplicateEmailException_Sets409Conflict()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new DuplicateEmailException();

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_OverlappingBookingException_Sets409Conflict()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new OverlappingBookingException();

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_KeyNotFoundException_Sets404NotFound()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new KeyNotFoundException("Item not found");

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_UnauthorizedAccessException_Sets401Unauthorized()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new UnauthorizedAccessException("Forbidden action");

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentException_Sets400BadRequest()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ArgumentException("Invalid argument");

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_Sets500InternalServerError()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Something unexpected broke");

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WhenProblemDetailsServiceReturnsFalse_WritesFallbackProblemDetails()
    {
        // Arrange
        var handler = CreateHandler();
        var context = new DefaultHttpContext();
        var bodyStream = new MemoryStream();
        context.Response.Body = bodyStream;
        var exception = new InvalidOperationException("Fatal crash");

        _problemDetailsServiceMock
            .Setup(p => p.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(false);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.StartsWith("application/problem+json", context.Response.ContentType);

        bodyStream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(bodyStream);
        var json = await reader.ReadToEndAsync();
        var parsed = JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(parsed);
        Assert.Equal(500, parsed.Status);
        Assert.Equal("Internal Server Error", parsed.Title);
        Assert.Equal("An unexpected error occurred.", parsed.Detail);
    }
}
