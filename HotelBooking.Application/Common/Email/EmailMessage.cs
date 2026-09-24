namespace HotelBooking.Application.Common.Email;

public sealed class EmailMessage
{
    public string To { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public EmailAttachment? Attachment { get; init; }
}