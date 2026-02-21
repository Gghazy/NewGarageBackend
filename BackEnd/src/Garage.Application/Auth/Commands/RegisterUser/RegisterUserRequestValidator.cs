namespace Garage.Application.Auth.Commands.RegisterUser;

using FluentValidation;
using Garage.Contracts.Auth;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
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
                .WithMessage("Validation.Required")
            .MinimumLength(6)
                .WithErrorCode("Validation.PasswordTooShort")
                .WithMessage("Password must be at least 6 characters long")
            .Must(ContainUpperCase)
                .WithErrorCode("Validation.PasswordRequiresUpper")
                .WithMessage("Password must contain at least one uppercase letter")
            .Must(ContainLowerCase)
                .WithErrorCode("Validation.PasswordRequiresLower")
                .WithMessage("Password must contain at least one lowercase letter")
            .Must(ContainDigit)
                .WithErrorCode("Validation.PasswordRequiresDigit")
                .WithMessage("Password must contain at least one digit");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithErrorCode("Validation.InvalidPhone")
                .WithMessage("Validation.InvalidFormat")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }

    private static bool ContainUpperCase(string password) => password.Any(char.IsUpper);
    private static bool ContainLowerCase(string password) => password.Any(char.IsLower);
    private static bool ContainDigit(string password) => password.Any(char.IsDigit);
}
