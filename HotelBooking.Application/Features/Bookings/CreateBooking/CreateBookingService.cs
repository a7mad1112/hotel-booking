using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Deals;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingService : IScopedService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IDealRepository _dealRepository;
    private readonly ILogger<CreateBookingService> _logger;

    public CreateBookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IDealRepository dealRepository,
        ILogger<CreateBookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _dealRepository = dealRepository;
        _logger = logger;
    }

    public async Task<ResultOfT<CreateBookingResponse>> CreateAsync(
        CreateBookingRequest request,
        int currentUserId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating booking for UserId {UserId}, RoomId {RoomId}, CheckIn {CheckInDate}, CheckOut {CheckOutDate}",
            currentUserId,
            request.RoomId,
            request.CheckInDate,
            request.CheckOutDate);

        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);

        if (room is null)
        {
            _logger.LogWarning(
                "Booking creation failed because RoomId {RoomId} was not found. UserId {UserId}",
                request.RoomId,
                currentUserId);

            return ResultOfT<CreateBookingResponse>.Failure("Room not found.");
        }

        if (!room.Availability)
        {
            _logger.LogWarning("Booking creation failed because RoomId {RoomId} is unavailable. UserId {UserId}",
                request.RoomId,
                currentUserId);

            return ResultOfT<CreateBookingResponse>.Failure("Room is not available.");
        }

        var hasOverlap = await _bookingRepository.HasOverlappingBookingAsync(
                request.RoomId,
                request.CheckInDate,
                request.CheckOutDate,
                cancellationToken);

        if (hasOverlap)
        {
            _logger.LogWarning("Booking creation failed because RoomId {RoomId} has an overlapping booking. UserId {UserId}",
                request.RoomId,
                currentUserId);

            return ResultOfT<CreateBookingResponse>.Failure("Room is not available for the selected dates.");
        }

        var nights = (request.CheckOutDate.Date - request.CheckInDate.Date).Days;

        var subtotal = room.PricePerNight * nights;

        var deal = await _dealRepository.GetApplicableDealAsync(
            room.HotelId,
            request.CheckInDate,
            request.CheckOutDate,
            cancellationToken);

        var discountAmount = 0m;

        if (deal is not null)
        {
            discountAmount = subtotal * deal.DiscountPercentage / 100m;

            _logger.LogInformation(
                "Deal applied to booking. HotelId {HotelId}, DiscountPercentage {DiscountPercentage}",
                room.HotelId,
                deal.DiscountPercentage);
        }

        var totalPrice = subtotal - discountAmount;

        var booking = new Booking
        {
            UserId = currentUserId,
            RoomId = room.Id,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);

        await _bookingRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Booking created successfully. BookingId {BookingId}, UserId {UserId}, RoomId {RoomId}, TotalPrice {TotalPrice}",
            booking.Id,
            currentUserId,
            room.Id,
            booking.TotalPrice);

        return ResultOfT<CreateBookingResponse>.Success(
            new CreateBookingResponse
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                HotelId = room.HotelId,

                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,

                Nights = nights,

                PricePerNight = room.PricePerNight,

                Subtotal = subtotal,

                DiscountPercentage =
                    deal?.DiscountPercentage,

                DiscountAmount = discountAmount,

                TotalPrice = booking.TotalPrice,

                Status = booking.Status
            });
    }
}