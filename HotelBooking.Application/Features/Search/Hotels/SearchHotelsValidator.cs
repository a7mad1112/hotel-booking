using FluentValidation;

namespace HotelBooking.Application.Features.Search.Hotels;

public sealed class SearchHotelsValidator
    : AbstractValidator<SearchHotelsRequest>
{
    public SearchHotelsValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        RuleFor(x => x.CityId)
            .GreaterThan(0)
            .When(x => x.CityId.HasValue);

        RuleFor(x => x.CheckInDate)
            .Must(date => date!.Value.Date >= DateTime.UtcNow.Date)
            .When(x => x.CheckInDate.HasValue);

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .When(x => x.CheckOutDate.HasValue && x.CheckInDate.HasValue);

        RuleFor(x => x.Adults)
            .GreaterThan(0)
            .When(x => x.Adults.HasValue);

        RuleFor(x => x.Children)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Children.HasValue);

        RuleFor(x => x.Rooms)
            .GreaterThan(0)
            .When(x => x.Rooms.HasValue);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue);

        RuleFor(x => x.MinStarRating)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinStarRating.HasValue);

        RuleFor(x => x.MaxStarRating)
            .GreaterThanOrEqualTo(x => x.MinStarRating)
            .When(x => x.MaxStarRating.HasValue && x.MinStarRating.HasValue);

        RuleForEach(x => x.AmenityIds)
            .GreaterThan(0);
    }
}