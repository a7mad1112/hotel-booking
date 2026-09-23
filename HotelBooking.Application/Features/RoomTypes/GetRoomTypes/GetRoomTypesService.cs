using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms.GetRooms;
using HotelBooking.Application.Features.RoomTypes;

namespace HotelBooking.Application.Features.RoomTypes.GetRoomTypes;

public sealed class GetRoomTypesService : IScopedService
{
    private readonly IRoomTypeRepository _repository;

    public GetRoomTypesService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetRoomsResponse>> GetAllAsync(PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<GetRoomsResponse>
        {
            Items = result.Items.Select(room => new GetRoomsResponse
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName = room.Hotel.Name,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType.Name,
                RoomNumber = room.RoomNumber,
                PricePerNight = room.PricePerNight,
                AdultsCapacity = room.AdultsCapacity,
                ChildrenCapacity = room.ChildrenCapacity,
                Availability = room.Availability
            }).ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount
        };
    }
}