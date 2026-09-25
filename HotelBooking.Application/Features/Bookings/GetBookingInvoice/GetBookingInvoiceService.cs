using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Bookings.GetBookingInvoice;

public sealed class GetBookingInvoiceService : IScopedService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IInvoiceGenerator _invoiceGenerator;

    public GetBookingInvoiceService(
        IBookingRepository bookingRepository,
        IInvoiceGenerator invoiceGenerator)
    {
        _bookingRepository = bookingRepository;
        _invoiceGenerator = invoiceGenerator;
    }

    public async Task<ResultOfT<InvoiceResult>> GetInvoiceAsync(
        int bookingId,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingForInvoiceAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return ResultOfT<InvoiceResult>.Failure("Booking not found.");
        }

        if (booking.UserId != currentUserId && !isAdmin)
        {
            return ResultOfT<InvoiceResult>.Failure("You are not allowed to access this invoice.");
        }

        var invoice = await _invoiceGenerator.GenerateAsync(booking, cancellationToken);

        return ResultOfT<InvoiceResult>.Success(invoice);
    }
}
