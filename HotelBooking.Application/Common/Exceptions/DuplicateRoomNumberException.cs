namespace HotelBooking.Application.Common.Exceptions;

public sealed class DuplicateRoomNumberException : Exception
{
    public DuplicateRoomNumberException()
        : base("A room with the same room number already exists in this hotel.")
    {
    }
}