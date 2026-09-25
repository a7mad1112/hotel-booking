using HotelBooking.Application.Common.Invoicing;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Bookings.GetBookingInvoice;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Bookings.GetBookingInvoice;

public class GetBookingInvoiceServiceTests
{
    [Fact]
    public async Task GetInvoiceAsync_Owner_ReturnsInvoice()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var invoiceGen = new Mock<IInvoiceGenerator>();

        var booking = new Booking { Id = 1, UserId = 10 };
        bookingRepo
            .Setup(x => x.GetBookingForInvoiceAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var expectedInvoice = new InvoiceResult
        {
            FileName = "booking-1-invoice.pdf",
            ContentType = "application/pdf",
            Content = [1, 2, 3]
        };

        invoiceGen
            .Setup(x => x.GenerateAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedInvoice);

        var service = new GetBookingInvoiceService(bookingRepo.Object, invoiceGen.Object);

        var result = await service.GetInvoiceAsync(1, 10, false, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("booking-1-invoice.pdf", result.Value!.FileName);
        Assert.Equal("application/pdf", result.Value.ContentType);
        Assert.Equal(expectedInvoice.Content, result.Value.Content);
    }

    [Fact]
    public async Task GetInvoiceAsync_Admin_ReturnsInvoice()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var invoiceGen = new Mock<IInvoiceGenerator>();

        var booking = new Booking { Id = 1, UserId = 10 };
        bookingRepo
            .Setup(x => x.GetBookingForInvoiceAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var expectedInvoice = new InvoiceResult
        {
            FileName = "booking-1-invoice.pdf",
            ContentType = "application/pdf",
            Content = [1, 2, 3]
        };

        invoiceGen
            .Setup(x => x.GenerateAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedInvoice);

        var service = new GetBookingInvoiceService(bookingRepo.Object, invoiceGen.Object);

        var result = await service.GetInvoiceAsync(1, 999, true, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("booking-1-invoice.pdf", result.Value!.FileName);
    }

    [Fact]
    public async Task GetInvoiceAsync_Unauthorized_ReturnsFailure()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var invoiceGen = new Mock<IInvoiceGenerator>();

        var booking = new Booking { Id = 1, UserId = 10 };
        bookingRepo
            .Setup(x => x.GetBookingForInvoiceAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var service = new GetBookingInvoiceService(bookingRepo.Object, invoiceGen.Object);

        var result = await service.GetInvoiceAsync(1, 999, false, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("You are not allowed to access this invoice.", result.Error);
        invoiceGen.Verify(x => x.GenerateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInvoiceAsync_NotFound_ReturnsFailure()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var invoiceGen = new Mock<IInvoiceGenerator>();

        bookingRepo
            .Setup(x => x.GetBookingForInvoiceAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var service = new GetBookingInvoiceService(bookingRepo.Object, invoiceGen.Object);

        var result = await service.GetInvoiceAsync(99, 10, false, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Booking not found.", result.Error);
    }
}
