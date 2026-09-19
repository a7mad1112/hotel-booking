namespace HotelBooking.API.Models;

public sealed class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}