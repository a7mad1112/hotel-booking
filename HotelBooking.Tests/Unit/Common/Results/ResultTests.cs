using HotelBooking.Application.Common.Results;

namespace HotelBooking.Tests.Unit.Common.Results;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal(ErrorType.Failure, result.ErrorType); // default None has Failure type
    }

    [Fact]
    public void Failure_WithDefaultString_DeterminesAppropriateType()
    {
        var notFoundResult = Result.Failure("Item not found.");
        Assert.False(notFoundResult.IsSuccess);
        Assert.Equal("Item not found.", notFoundResult.Error);
        Assert.Equal(ErrorType.NotFound, notFoundResult.ErrorType);

        var forbiddenResult = Result.Failure("You are not allowed to update this item.");
        Assert.False(forbiddenResult.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, forbiddenResult.ErrorType);

        var conflictResult = Result.Failure("An item with the same name already exists.");
        Assert.False(conflictResult.IsSuccess);
        Assert.Equal(ErrorType.Conflict, conflictResult.ErrorType);

        var genericResult = Result.Failure("Invalid parameter.");
        Assert.False(genericResult.IsSuccess);
        Assert.Equal(ErrorType.Failure, genericResult.ErrorType);
    }

    [Fact]
    public void ExplicitFactoryMethods_SetCorrectProperties()
    {
        var notFound = Result.NotFound("Hotel not found.");
        Assert.False(notFound.IsSuccess);
        Assert.Equal("Hotel not found.", notFound.Error);
        Assert.Equal(ErrorType.NotFound, notFound.ErrorType);

        var forbidden = Result.Forbidden("Forbidden.");
        Assert.False(forbidden.IsSuccess);
        Assert.Equal("Forbidden.", forbidden.Error);
        Assert.Equal(ErrorType.Forbidden, forbidden.ErrorType);

        var conflict = Result.Conflict("Conflict occurred.");
        Assert.False(conflict.IsSuccess);
        Assert.Equal("Conflict occurred.", conflict.Error);
        Assert.Equal(ErrorType.Conflict, conflict.ErrorType);

        var validation = Result.Validation("Validation failed.");
        Assert.False(validation.IsSuccess);
        Assert.Equal("Validation failed.", validation.Error);
        Assert.Equal(ErrorType.Validation, validation.ErrorType);

        var unauthorized = Result.Unauthorized("Unauthorized access.");
        Assert.False(unauthorized.IsSuccess);
        Assert.Equal("Unauthorized access.", unauthorized.Error);
        Assert.Equal(ErrorType.Unauthorized, unauthorized.ErrorType);
    }

    [Fact]
    public void ResultOfT_Success_ReturnsValue()
    {
        var result = ResultOfT<string>.Success("test-value");

        Assert.True(result.IsSuccess);
        Assert.Equal("test-value", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void ResultOfT_TypedFailures_SetCorrectProperties()
    {
        var notFound = ResultOfT<int>.NotFound("Not found.");
        Assert.False(notFound.IsSuccess);
        Assert.Equal(default, notFound.Value);
        Assert.Equal("Not found.", notFound.Error);
        Assert.Equal(ErrorType.NotFound, notFound.ErrorType);

        var forbidden = ResultOfT<int>.Forbidden("Not allowed.");
        Assert.False(forbidden.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, forbidden.ErrorType);

        var conflict = ResultOfT<int>.Conflict("Already exists.");
        Assert.False(conflict.IsSuccess);
        Assert.Equal(ErrorType.Conflict, conflict.ErrorType);

        var validation = ResultOfT<int>.Validation("Invalid field.");
        Assert.False(validation.IsSuccess);
        Assert.Equal(ErrorType.Validation, validation.ErrorType);
    }
}
