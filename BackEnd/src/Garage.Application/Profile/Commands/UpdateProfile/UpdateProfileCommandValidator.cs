using FluentValidation;

namespace Garage.Application.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.Request.NameAr)
            .NotEmpty().WithMessage("Validation.Required")
            .MinimumLength(2).WithMessage("Name (Arabic) must be at least 2 characters")
            .MaximumLength(200).WithMessage("Name (Arabic) must not exceed 200 characters");

        RuleFor(x => x.Request.NameEn)
            .NotEmpty().WithMessage("Validation.Required")
            .MinimumLength(2).WithMessage("Name (English) must be at least 2 characters")
            .MaximumLength(200).WithMessage("Name (English) must not exceed 200 characters");

        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Validation.Required")
            .EmailAddress().WithMessage("Validation.InvalidEmail");
    }
}
