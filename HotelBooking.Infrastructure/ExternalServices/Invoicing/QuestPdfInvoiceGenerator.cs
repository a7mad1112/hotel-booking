using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelBooking.Infrastructure.ExternalServices.Invoicing;

public sealed class QuestPdfInvoiceGenerator
    : IInvoiceGenerator, IScopedService
{
    public Task<InvoiceResult> GenerateAsync(Booking booking, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var nights = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;

        var pricePerNight = nights > 0
            ? booking.TotalPrice / nights
            : booking.TotalPrice;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("Hotel Booking")
                            .FontSize(24)
                            .Bold();

                        column.Item()
                            .Text("Booking Confirmation & Invoice")
                            .FontSize(14);
                    });

                page.Content()
                    .PaddingVertical(25)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item()
                            .Text($"Booking #{booking.Id}")
                            .FontSize(18)
                            .Bold();

                        column.Item()
                            .Text($"Customer: {booking.User.Email}");

                        column.Item()
                            .Text(
                                $"Hotel: {booking.Room.Hotel.Name}");

                        column.Item()
                            .Text(
                                $"City: {booking.Room.Hotel.City.Name}");

                        column.Item()
                            .Text(
                                $"Room: {booking.Room.RoomNumber}");

                        column.Item()
                            .Text(
                                $"Check-in: {booking.CheckInDate:yyyy-MM-dd}");

                        column.Item()
                            .Text(
                                $"Check-out: {booking.CheckOutDate:yyyy-MM-dd}");

                        column.Item()
                            .Text($"Nights: {nights}");

                        column.Item()
                            .PaddingTop(15)
                            .LineHorizontal(1);

                        column.Item()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text("Price per night");

                                row.AutoItem()
                                    .Text($"{pricePerNight:F2}");
                            });

                        column.Item()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text("Total");

                                row.AutoItem()
                                    .Text($"{booking.TotalPrice:F2}")
                                    .Bold();
                            });

                        column.Item()
                            .PaddingTop(15)
                            .Text("Payment status: Paid")
                            .Bold();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Booking #");
                        text.Span(booking.Id.ToString());
                        text.Span(" • ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });

        var pdf = document.GeneratePdf();

        return Task.FromResult(
            new InvoiceResult
            {
                FileName = $"booking-{booking.Id}-invoice.pdf",
                ContentType = "application/pdf",
                Content = pdf
            });
    }
}