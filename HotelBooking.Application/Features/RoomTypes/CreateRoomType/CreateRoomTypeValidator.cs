using FluentValidation;

namespace HotelBooking.Application.Features.RoomTypes.CreateRoomType;

public sealed class CreateRoomTypeValidator : AbstractValidator<CreateRoomTypeRequest>
{
    public CreateRoomTypeValidator()
    {
        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .MaximumLength(100);
    }
}