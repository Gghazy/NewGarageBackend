namespace Garage.Application.Auth.Commands.Login;

using FluentValidation;
using Garage.Contracts.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .EmailAddress()
                .WithErrorCode("Validation.InvalidEmail")
                .WithMessage("Validation.InvalidEmail");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");
    }
}
