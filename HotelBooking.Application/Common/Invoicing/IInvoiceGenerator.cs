using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Common.Invoicing;

public interface IInvoiceGenerator
{
    Task<InvoiceResult> GenerateAsync(Booking booking, CancellationToken cancellationToken);
}