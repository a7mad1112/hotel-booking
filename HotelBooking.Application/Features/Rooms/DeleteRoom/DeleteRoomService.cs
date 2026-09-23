using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;

namespace HotelBooking.Application.Features.Rooms.DeleteRoom;

public sealed class DeleteRoomService : IScopedService
{
    private readonly IRoomRepository _repository;

    public DeleteRoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> DeleteAsync(int id, int currentUserId, bool isAdmin, CancellationToken cancellationToken)
    {
        var room = await _repository.GetDetailsByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return Result.Failure("Room not found.");
        }

        if (!isAdmin && room.Hotel.OwnerId != currentUserId)
        {
            return Result.Failure("You are not allowed to delete this room.");
        }

        var hasBookings = await _repository.HasBookingsAsync(id, cancellationToken);

        if (hasBookings)
        {
            return Result.Failure("Cannot delete a room that has related bookings.");
        }

        _repository.Delete(room);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}