using FluentValidation;

namespace HotelBooking.Application.Features.Deals.UpdateDeal;

public sealed class UpdateDealValidator : AbstractValidator<UpdateDealRequest>
{
    public UpdateDealValidator()
    {
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