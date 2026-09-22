using FluentValidation;

namespace HotelBooking.Application.Features.Hotels.UpdateHotel;

public sealed class UpdateHotelValidator
    : AbstractValidator<UpdateHotelRequest>
{
    public UpdateHotelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CityId)
            .GreaterThan(0);

        RuleFor(x => x.StarRating)
            .InclusiveBetween(0, 5);

        RuleFor(x => x.Location)
            .NotEmpty();
    }
}