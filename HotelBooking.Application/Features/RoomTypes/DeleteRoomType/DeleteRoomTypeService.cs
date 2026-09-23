using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.RoomTypes;

namespace HotelBooking.Application.Features.RoomTypes.DeleteRoomType;

public sealed class DeleteRoomTypeService : IScopedService
{
    private readonly IRoomTypeRepository _repository;

    public DeleteRoomTypeService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var roomType = await _repository.GetByIdAsync(id, cancellationToken);

        if (roomType is null)
        {
            return Result.Failure("Room type not found.");
        }

        var hasRooms = await _repository.HasRoomsAsync(id, cancellationToken);

        if (hasRooms)
        {
            return Result.Failure("Cannot delete a room type that is used by rooms.");
        }

        _repository.Delete(roomType);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}