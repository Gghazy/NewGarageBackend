namespace Garage.Application.MechIssues.Commands.Create;

using FluentValidation;
using Garage.Contracts.MechIssues;

public class MechIssueRequestValidator : AbstractValidator<MechIssueRequest>
{
    public MechIssueRequestValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (Arabic) must be at least 2 characters")
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (Arabic) must not exceed 200 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (English) must be at least 2 characters")
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (English) must not exceed 200 characters");

        RuleFor(x => x.MechIssueTypeId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");
    }
}
