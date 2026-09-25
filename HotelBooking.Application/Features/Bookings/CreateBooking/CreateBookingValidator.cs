using FluentValidation;

namespace HotelBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0);

        RuleFor(x => x.CheckInDate)
            .NotEmpty();

        RuleFor(x => x.CheckOutDate)
            .NotEmpty()
            .GreaterThan(x => x.CheckInDate);

        RuleFor(x => x.SpecialRequests)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.SpecialRequests));
    }
}