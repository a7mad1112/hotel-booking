using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Rooms;

namespace HotelBooking.Application.Features.Rooms.GetRooms;

public sealed class GetRoomsService : IScopedService
{
    private readonly IRoomRepository _repository;

    public GetRoomsService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetRoomsResponse>> GetAllAsync(PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<GetRoomsResponse>
        {
            Items = result.Items.Select(room => new GetRoomsResponse
                {
                    Id = room.Id,
                    HotelId = room.HotelId,
                    HotelName = room.Hotel.Name,
                    RoomNumber = room.RoomNumber,
                    RoomTypeId = room.RoomTypeId,
                    RoomTypeName = room.RoomType.Name,
                    PricePerNight = room.PricePerNight,
                    AdultsCapacity = room.AdultsCapacity,
                    ChildrenCapacity = room.ChildrenCapacity,
                    Availability = room.Availability
                })
                .ToList(),

            Page = request.Page,

            PageSize = request.PageSize,

            TotalCount = result.TotalCount
        };
    }
}