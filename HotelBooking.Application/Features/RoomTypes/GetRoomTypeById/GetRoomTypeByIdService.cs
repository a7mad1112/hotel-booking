using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Results;
using HotelBooking.Application.Features.RoomTypes;

namespace HotelBooking.Application.Features.RoomTypes.GetRoomTypeById;

public sealed class GetRoomTypeByIdService : IScopedService
{
    private readonly IRoomTypeRepository _repository;

    public GetRoomTypeByIdService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOfT<GetRoomTypeByIdResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var roomType = await _repository.GetByIdAsync(id, cancellationToken);

        if (roomType is null)
        {
            return ResultOfT<GetRoomTypeByIdResponse>.Failure("Room type not found.");
        }

        return ResultOfT<GetRoomTypeByIdResponse>.Success(
            new GetRoomTypeByIdResponse
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            });
    }
}