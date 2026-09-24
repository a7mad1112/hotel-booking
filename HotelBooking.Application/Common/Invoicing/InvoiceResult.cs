namespace HotelBooking.Application.Common.Invoicing;

public sealed class InvoiceResult
{
    public string FileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = "application/pdf";

    public byte[] Content { get; init; } = [];
}