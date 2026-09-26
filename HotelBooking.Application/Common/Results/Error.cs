namespace HotelBooking.Application.Common.Results;

public sealed record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Failure(string description, string code = "General.Failure") =>
        new(code, description, DetermineErrorType(description, ErrorType.Failure));

    public static Error Validation(string description, string code = "General.Validation") =>
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string description, string code = "General.NotFound") =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string description, string code = "General.Conflict") =>
        new(code, description, ErrorType.Conflict);

    public static Error Forbidden(string description, string code = "General.Forbidden") =>
        new(code, description, ErrorType.Forbidden);

    public static Error Unauthorized(string description, string code = "General.Unauthorized") =>
        new(code, description, ErrorType.Unauthorized);

    internal static ErrorType DetermineErrorType(string description, ErrorType defaultType)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return defaultType;
        }

        if (description.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return ErrorType.NotFound;
        }

        if (description.Contains("not allowed", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("forbidden", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("Only guests who have booked", StringComparison.OrdinalIgnoreCase))
        {
            return ErrorType.Forbidden;
        }

        if (description.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("already been", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("is not available", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("cannot delete", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("conflict", StringComparison.OrdinalIgnoreCase))
        {
            return ErrorType.Conflict;
        }

        if (description.Contains("invalid email or password", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return ErrorType.Unauthorized;
        }

        return defaultType;
    }

    public static implicit operator string(Error error) => error.Description;
}
