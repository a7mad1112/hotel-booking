using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pricing;
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
    private readonly IPricingCalculator _pricingCalculator;
    private readonly ICurrentUserService? _currentUserService;

    public CreateBookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IDealRepository dealRepository,
        ILogger<CreateBookingService> logger)
        : this(bookingRepository, roomRepository, dealRepository, logger, new PricingCalculator(), null)
    {
    }

    public CreateBookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IDealRepository dealRepository,
        ILogger<CreateBookingService> logger,
        IPricingCalculator pricingCalculator)
        : this(bookingRepository, roomRepository, dealRepository, logger, pricingCalculator, null)
    {
    }

    public CreateBookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IDealRepository dealRepository,
        ILogger<CreateBookingService> logger,
        IPricingCalculator pricingCalculator,
        ICurrentUserService? currentUserService)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _dealRepository = dealRepository;
        _logger = logger;
        _pricingCalculator = pricingCalculator;
        _currentUserService = currentUserService;
    }

    public Task<ResultOfT<CreateBookingResponse>> CreateAsync(
        CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService?.UserId ?? 0;
        return CreateAsync(request, currentUserId, cancellationToken);
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

        var deal = await _dealRepository.GetApplicableDealAsync(
            room.HotelId,
            request.CheckInDate,
            request.CheckOutDate,
            cancellationToken);

        if (deal is not null)
        {
            _logger.LogInformation(
                "Deal applied to booking. HotelId {HotelId}, DiscountPercentage {DiscountPercentage}",
                room.HotelId,
                deal.DiscountPercentage);
        }

        var pricing = _pricingCalculator.CalculatePrice(
            room.PricePerNight,
            request.CheckInDate,
            request.CheckOutDate,
            deal?.DiscountPercentage);

        var booking = new Booking
        {
            UserId = currentUserId,
            RoomId = room.Id,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalPrice = pricing.TotalPrice,
            Status = BookingStatus.Pending,
            SpecialRequests = string.IsNullOrWhiteSpace(request.SpecialRequests)
                ? null
                : request.SpecialRequests.Trim()
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);

        try
        {
            await _bookingRepository.SaveChangesAsync(cancellationToken);
        }
        catch (OverlappingBookingException)
        {
            _logger.LogWarning(
                "Booking creation failed due to overlapping booking conflict during persistence. RoomId {RoomId}, UserId {UserId}",
                room.Id,
                currentUserId);

            return ResultOfT<CreateBookingResponse>.Failure("Room is not available for the selected dates.");
        }

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

                Nights = pricing.Nights,

                PricePerNight = pricing.PricePerNight,

                Subtotal = pricing.Subtotal,

                DiscountPercentage = pricing.DiscountPercentage,

                DiscountAmount = pricing.DiscountAmount,

                TotalPrice = booking.TotalPrice,

                Status = booking.Status,

                SpecialRequests = booking.SpecialRequests
            });
    }
}