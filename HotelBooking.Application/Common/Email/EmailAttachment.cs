namespace HotelBooking.Application.Common.Email;

public sealed class EmailAttachment
{
    public string FileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public byte[] Content { get; init; } = [];
}