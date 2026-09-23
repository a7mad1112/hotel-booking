using FluentValidation;

namespace HotelBooking.Application.Features.RoomTypes.UpdateRoomType;

public sealed class UpdateRoomTypeValidator : AbstractValidator<UpdateRoomTypeRequest>
{
    public UpdateRoomTypeValidator()
    {
        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .MaximumLength(100);
    }
}