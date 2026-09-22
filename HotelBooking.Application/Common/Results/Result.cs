namespace HotelBooking.Application.Common.Results;

public sealed class Result
{
    private Result(
        bool isSuccess,
        string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public string? Error { get; }


    public static Result Success()
    {
        return new Result(true, null);
    }


    public static Result Failure(string error)
    {
        return new Result(false, error);
    }
}