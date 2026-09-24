using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Payments;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Payments.CreatePayment;

public sealed class CreatePaymentService : IScopedService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentProvider _paymentProvider;

    public CreatePaymentService(IBookingRepository bookingRepository, IPaymentRepository paymentRepository,
        IPaymentProvider paymentProvider)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _paymentProvider = paymentProvider;
    }

    public async Task<ResultOfT<CreatePaymentResponse>> CreateAsync(
        int bookingId,
        int currentUserId,
        string idempotencyKey,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return ResultOfT<CreatePaymentResponse>.Failure("Idempotency key is required.");
        }

        var existingPayment = await _paymentRepository.GetByIdempotencyKeyAsync(
            idempotencyKey,
            currentUserId,
            cancellationToken);

        if (existingPayment is not null)
        {
            if (existingPayment.BookingId != bookingId)
            {
                return ResultOfT<CreatePaymentResponse>.Failure("The idempotency key has already been used.");
            }

            return ResultOfT<CreatePaymentResponse>.Success(MapResponse(existingPayment));
        }

        var booking = await _bookingRepository.GetCheckoutAsync(bookingId, currentUserId, cancellationToken);

        if (booking is null)
        {
            return ResultOfT<CreatePaymentResponse>.Failure("Booking not found.");
        }

        if (booking.Status != BookingStatus.Pending)
        {
            return ResultOfT<CreatePaymentResponse>.Failure("Booking is not available for payment.");
        }

        var existingBookingPayment =
            await _paymentRepository.GetByBookingIdAsync(bookingId, currentUserId, cancellationToken);

        if (existingBookingPayment is not null)
        {
            return ResultOfT<CreatePaymentResponse>.Failure("A payment already exists for this booking.");
        }

        var checkoutResult = await _paymentProvider.CreateCheckoutSessionAsync(
            new PaymentCheckoutRequest
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                Currency = "usd",
                CustomerEmail = booking.User.Email,
                Description =
                    $"Hotel booking #{booking.Id} - {booking.Room.Hotel.Name}",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                IdempotencyKey = idempotencyKey
            },
            cancellationToken);

        var payment = new Payment
        {
            BookingId = booking.Id,
            Provider = _paymentProvider.Name,
            TransactionId = checkoutResult.TransactionId,
            PaymentIntentId = checkoutResult.PaymentIntentId,
            IdempotencyKey = idempotencyKey,
            CheckoutUrl = checkoutResult.CheckoutUrl,
            Amount = booking.TotalPrice,
            Status = PaymentStatus.Pending,
            PaymentDate = DateTimeOffset.UtcNow
        };

        await _paymentRepository.AddAsync(payment,
            cancellationToken);

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreatePaymentResponse>.Success(MapResponse(payment));
    }

    private static CreatePaymentResponse MapResponse(Payment payment)
    {
        return new CreatePaymentResponse
        {
            BookingId = payment.BookingId,
            Provider = payment.Provider,
            TransactionId = payment.TransactionId,
            PaymentIntentId = payment.PaymentIntentId,
            Amount = payment.Amount,
            Status = payment.Status,
            CheckoutUrl = payment.CheckoutUrl
        };
    }
}