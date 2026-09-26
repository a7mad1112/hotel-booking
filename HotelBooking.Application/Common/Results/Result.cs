namespace HotelBooking.Application.Common.Results;

public sealed class Result
{
    private Result(
        bool isSuccess,
        Error? error)
    {
        IsSuccess = isSuccess;
        ErrorDetail = error ?? Results.Error.None;
        Error = isSuccess ? null : error?.Description;
    }

    public bool IsSuccess { get; }

    public string? Error { get; }

    public Error ErrorDetail { get; }

    public ErrorType ErrorType => ErrorDetail.Type;

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string error)
    {
        return new Result(false, Results.Error.Failure(error));
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result NotFound(string error = "Not found.")
    {
        return new Result(false, Results.Error.NotFound(error));
    }

    public static Result Conflict(string error = "Conflict.")
    {
        return new Result(false, Results.Error.Conflict(error));
    }

    public static Result Forbidden(string error = "Forbidden.")
    {
        return new Result(false, Results.Error.Forbidden(error));
    }

    public static Result Unauthorized(string error = "Unauthorized.")
    {
        return new Result(false, Results.Error.Unauthorized(error));
    }

    public static Result Validation(string error = "Validation error.")
    {
        return new Result(false, Results.Error.Validation(error));
    }
}