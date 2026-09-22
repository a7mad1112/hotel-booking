namespace HotelBooking.Application.Common.Results;

public sealed class ResultOfT<T>
{
    private ResultOfT(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static ResultOfT<T> Success(T value)
    {
        return new ResultOfT<T>(true, value, null);
    }

    public static ResultOfT<T> Failure(string? error)
    {
        return new ResultOfT<T>(false, default, error);
    }
}