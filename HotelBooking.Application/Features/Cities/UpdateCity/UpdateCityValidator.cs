using FluentValidation;

namespace HotelBooking.Application.Features.Cities.UpdateCity;

public sealed class UpdateCityValidator : AbstractValidator<UpdateCityRequest>
{
    public UpdateCityValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .MaximumLength(20);
    }
}