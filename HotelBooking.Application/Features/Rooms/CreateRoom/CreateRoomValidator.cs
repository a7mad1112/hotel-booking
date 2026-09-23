using FluentValidation;

namespace HotelBooking.Application.Features.Rooms.CreateRoom;

public sealed class CreateRoomValidator
    : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.HotelId)
            .GreaterThan(0);

        RuleFor(x => x.RoomNumber)
            .Must(roomNumber => !string.IsNullOrWhiteSpace(roomNumber))
            .MaximumLength(20);

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0);

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0);

        RuleFor(x => x.AdultsCapacity)
            .GreaterThan(0);

        RuleFor(x => x.ChildrenCapacity)
            .GreaterThanOrEqualTo(0);
    }
}