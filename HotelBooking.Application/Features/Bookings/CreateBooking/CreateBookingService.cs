using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Deals;
using HotelBooking.Application.Features.Rooms;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingService : IScopedService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IDealRepository _dealRepository;

    public CreateBookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository,
        IDealRepository dealRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _dealRepository = dealRepository;
    }

    public async Task<ResultOfT<CreateBookingResponse>> CreateAsync(
        CreateBookingRequest request,
        int currentUserId,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);

        if (room is null)
        {
            return ResultOfT<CreateBookingResponse>.Failure("Room not found.");
        }

        if (!room.Availability)
        {
            return ResultOfT<CreateBookingResponse>.Failure("Room is not available.");
        }

        var hasOverlap =
            await _bookingRepository.HasOverlappingBookingAsync(
                request.RoomId,
                request.CheckInDate,
                request.CheckOutDate,
                cancellationToken);

        if (hasOverlap)
        {
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