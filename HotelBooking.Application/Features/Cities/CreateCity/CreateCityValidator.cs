using FluentValidation;

namespace HotelBooking.Application.Features.Cities.CreateCity;

public sealed class CreateCityValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .MaximumLength(20);
    }
}