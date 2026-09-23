using FluentValidation;

namespace HotelBooking.Application.Features.Rooms.UpdateRoom;

public sealed class UpdateRoomValidator
    : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.RoomNumber)
            .Must(roomNumber => !string.IsNullOrWhiteSpace(roomNumber))
            .MaximumLength(20);

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0);

        RuleFor(x => x.PricePerNight)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AdultsCapacity)
            .GreaterThan(0);

        RuleFor(x => x.ChildrenCapacity)
            .GreaterThanOrEqualTo(0);
    }
}