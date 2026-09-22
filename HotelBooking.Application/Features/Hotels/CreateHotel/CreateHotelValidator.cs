using FluentValidation;

namespace HotelBooking.Application.Features.Hotels.CreateHotel;

public class CreateHotelValidator
    : AbstractValidator<CreateHotelRequest>
{
    public CreateHotelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);


        RuleFor(x => x.CityId)
            .GreaterThan(0);


        RuleFor(x => x.OwnerId)
            .GreaterThan(0);


        RuleFor(x => x.StarRating)
            .InclusiveBetween(0, 5);


        RuleFor(x => x.Location)
            .NotEmpty();
    }
}