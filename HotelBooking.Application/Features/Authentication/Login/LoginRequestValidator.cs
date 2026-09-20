using FluentValidation;

namespace HotelBooking.Application.Features.Authentication.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(r => r.Password)
            .NotEmpty();
    }
}