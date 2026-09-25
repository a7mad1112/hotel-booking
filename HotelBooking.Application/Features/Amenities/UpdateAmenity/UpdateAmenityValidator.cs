using FluentValidation;

namespace HotelBooking.Application.Features.Amenities.UpdateAmenity;

public sealed class UpdateAmenityValidator : AbstractValidator<UpdateAmenityRequest>
{
    public UpdateAmenityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
