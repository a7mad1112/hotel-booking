using FluentValidation;

namespace HotelBooking.Application.Features.Deals.CreateDeal;

public sealed class CreateDealValidator : AbstractValidator<CreateDealRequest>
{
    public CreateDealValidator()
    {
        RuleFor(x => x.HotelId)
            .GreaterThan(0);

        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate);
    }
}