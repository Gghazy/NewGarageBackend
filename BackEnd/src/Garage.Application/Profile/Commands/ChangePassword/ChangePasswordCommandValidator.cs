using FluentValidation;

namespace Garage.Application.Profile.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request.CurrentPassword)
            .NotEmpty().WithMessage("Validation.Required");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty().WithMessage("Validation.Required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Request.ConfirmPassword)
            .NotEmpty().WithMessage("Validation.Required")
            .Equal(x => x.Request.NewPassword).WithMessage("Passwords do not match");
    }
}
