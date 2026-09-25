namespace HotelBooking.Application.Common.Exceptions;

public sealed class OverlappingBookingException : Exception
{
    public OverlappingBookingException()
        : base("The room is not available for the selected dates.")
    {
    }
}
