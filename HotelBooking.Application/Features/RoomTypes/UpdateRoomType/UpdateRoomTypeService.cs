using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.RoomTypes;

namespace HotelBooking.Application.Features.RoomTypes.UpdateRoomType;

public sealed class UpdateRoomTypeService : IScopedService
{
    private readonly IRoomTypeRepository _repository;

    public UpdateRoomTypeService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<UpdateRoomTypeResponse>> UpdateAsync(int id, UpdateRoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var roomType = await _repository.GetByIdAsync(id, cancellationToken);

        if (roomType is null)
        {
            return ResultOfT<UpdateRoomTypeResponse>.Failure("Room type not found.");
        }

        roomType.Name = request.Name.Trim();
        roomType.Description = request.Description?.Trim();

        await _repository.SaveChangesAsync(cancellationToken);

        return ResultOfT<UpdateRoomTypeResponse>.Success(
            new UpdateRoomTypeResponse
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            });
    }
}