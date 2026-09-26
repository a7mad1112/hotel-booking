namespace HotelBooking.Application.Common.Results;

public sealed class ResultOfT<T>
{
    private ResultOfT(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorDetail = error ?? Results.Error.None;
        Error = isSuccess ? null : error?.Description;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public Error ErrorDetail { get; }
    public ErrorType ErrorType => ErrorDetail.Type;

    public static ResultOfT<T> Success(T value)
    {
        return new ResultOfT<T>(true, value, null);
    }

    public static ResultOfT<T> Failure(string? error)
    {
        return new ResultOfT<T>(false, default, error is null ? null : Results.Error.Failure(error));
    }

    public static ResultOfT<T> Failure(Error error)
    {
        return new ResultOfT<T>(false, default, error);
    }

    public static ResultOfT<T> NotFound(string error = "Not found.")
    {
        return new ResultOfT<T>(false, default, Results.Error.NotFound(error));
    }

    public static ResultOfT<T> Conflict(string error = "Conflict.")
    {
        return new ResultOfT<T>(false, default, Results.Error.Conflict(error));
    }

    public static ResultOfT<T> Forbidden(string error = "Forbidden.")
    {
        return new ResultOfT<T>(false, default, Results.Error.Forbidden(error));
    }

    public static ResultOfT<T> Unauthorized(string error = "Unauthorized.")
    {
        return new ResultOfT<T>(false, default, Results.Error.Unauthorized(error));
    }

    public static ResultOfT<T> Validation(string error = "Validation error.")
    {
        return new ResultOfT<T>(false, default, Results.Error.Validation(error));
    }
}