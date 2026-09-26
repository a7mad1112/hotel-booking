using HotelBooking.API.Extensions;
using HotelBooking.Application.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Tests.Unit.API.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public void ToActionResult_SuccessResult_ReturnsNoContent()
    {
        var result = Result.Success();

        var actionResult = result.ToActionResult();

        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public void ToActionResult_SuccessWithCallback_ReturnsCallbackResult()
    {
        var result = Result.Success();

        var actionResult = result.ToActionResult(() => new OkObjectResult("custom"));

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal("custom", okResult.Value);
    }

    [Fact]
    public void ToActionResult_ResultOfT_Success_ReturnsOkWithValue()
    {
        var result = ResultOfT<string>.Success("hello");

        var actionResult = result.ToActionResult();

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Equal("hello", okResult.Value);
    }

    [Fact]
    public void ToActionResult_ResultOfT_SuccessWithCustomCallback_ReturnsCustomResult()
    {
        var result = ResultOfT<string>.Success("hello");

        var actionResult = result.ToActionResult(val => new CreatedResult("/test", val));

        var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
        Assert.Equal("hello", createdResult.Value);
    }

    [Fact]
    public void ToActionResult_NotFound_Returns404WithCorrectMessage()
    {
        var result = Result.NotFound("Hotel not found.");

        var actionResult = result.ToActionResult();

        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        Assert.NotNull(notFound.Value);
    }

    [Fact]
    public void ToActionResult_Conflict_Returns409WithCorrectMessage()
    {
        var result = Result.Conflict("Name already exists.");

        var actionResult = result.ToActionResult();

        var conflict = Assert.IsType<ConflictObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
    }

    [Fact]
    public void ToActionResult_Forbidden_Returns403WithCorrectMessage()
    {
        var result = Result.Forbidden("Not allowed.");

        var actionResult = result.ToActionResult();

        var forbidden = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status403Forbidden, forbidden.StatusCode);
    }

    [Fact]
    public void ToActionResult_Validation_Returns400()
    {
        var result = Result.Validation("Invalid input.");

        var actionResult = result.ToActionResult();

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public void ToActionResult_Unauthorized_Returns401()
    {
        var result = Result.Unauthorized("Invalid credentials.");

        var actionResult = result.ToActionResult();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
    }

    [Fact]
    public void ToActionResult_GenericFailure_Returns400()
    {
        var result = Result.Failure("General failure.");

        var actionResult = result.ToActionResult();

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public void ToErrorResult_ForGenericResult_ReturnsTypedActionResult()
    {
        var notFound = ResultOfT<int>.NotFound("Entity not found.");

        var actionResult = notFound.ToErrorResult();

        Assert.IsType<NotFoundObjectResult>(actionResult);
    }
}
