using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Rooms.CreateRoom;

public sealed class CreateRoomService : IScopedService
{
    private readonly IRoomRepository _repository;
    private readonly IHotelRepository _hotelRepository;

    public CreateRoomService(IRoomRepository repository, IHotelRepository hotelRepository)
    {
        _repository = repository;
        _hotelRepository = hotelRepository;
    }

    public async Task<ResultOfT<CreateRoomResponse>> CreateAsync(
        CreateRoomRequest request,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);

        if (hotel is null)
        {
            return ResultOfT<CreateRoomResponse>.Failure("Hotel not found.");
        }

        if (!isAdmin && hotel.OwnerId != currentUserId)
        {
            return ResultOfT<CreateRoomResponse>.Failure("You are not allowed to create rooms for this hotel.");
        }

        var roomTypeExists = await _repository.RoomTypeExistsAsync(request.RoomTypeId, cancellationToken);

        if (!roomTypeExists)
        {
            return ResultOfT<CreateRoomResponse>.Failure("Room type not found.");
        }

        var room = new Room
        {
            HotelId = request.HotelId,
            RoomNumber = request.RoomNumber.Trim(),
            RoomTypeId = request.RoomTypeId,
            PricePerNight = request.PricePerNight,
            AdultsCapacity = request.AdultsCapacity,
            ChildrenCapacity = request.ChildrenCapacity,
            Availability = request.Availability
        };

        await _repository.AddAsync(room, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<CreateRoomResponse>.Success(
            new CreateRoomResponse
            {
                Id = room.Id,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                RoomTypeId = room.RoomTypeId,
                PricePerNight = room.PricePerNight,
                AdultsCapacity = room.AdultsCapacity,
                ChildrenCapacity = room.ChildrenCapacity,
                Availability = room.Availability
            });
    }
}