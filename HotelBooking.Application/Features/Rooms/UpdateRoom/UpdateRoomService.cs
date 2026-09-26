using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.Rooms;

namespace HotelBooking.Application.Features.Rooms.UpdateRoom;

public sealed class UpdateRoomService : IScopedService
{
    private readonly IRoomRepository _repository;

    public UpdateRoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<UpdateRoomResponse>> UpdateAsync(
        int id,
        UpdateRoomRequest request,
        int currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var room = await _repository.GetDetailsByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return ResultOfT<UpdateRoomResponse>.NotFound("Room not found.");
        }

        if (!isAdmin && room.Hotel.OwnerId != currentUserId)
        {
            return ResultOfT<UpdateRoomResponse>.Forbidden("You are not allowed to update this room.");
        }

        var roomTypeExists = await _repository.RoomTypeExistsAsync(request.RoomTypeId, cancellationToken);

        if (!roomTypeExists)
        {
            return ResultOfT<UpdateRoomResponse>.Validation("Room type not found.");
        }

        room.RoomTypeId = request.RoomTypeId;
        room.RoomNumber = request.RoomNumber.Trim();
        room.PricePerNight = request.PricePerNight;
        room.AdultsCapacity = request.AdultsCapacity;
        room.ChildrenCapacity = request.ChildrenCapacity;
        room.Availability = request.Availability;

        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<UpdateRoomResponse>.Success(new UpdateRoomResponse
        {
            Id = room.Id,
            HotelId = room.HotelId,
            RoomTypeId = room.RoomTypeId,
            RoomNumber = room.RoomNumber,
            PricePerNight = room.PricePerNight,
            AdultsCapacity = room.AdultsCapacity,
            ChildrenCapacity = room.ChildrenCapacity,
            Availability = room.Availability
        });
    }
}