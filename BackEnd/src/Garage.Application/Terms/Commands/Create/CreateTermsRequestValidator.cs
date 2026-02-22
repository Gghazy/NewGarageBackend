namespace Garage.Application.Terms.Commands.Create;

using FluentValidation;
using Garage.Contracts.Terms;

public class CreateTermsRequestValidator : AbstractValidator<CreateTermsRequest>
{
    public CreateTermsRequestValidator()
    {
        RuleFor(x => x.TermsAndCondtionsAr)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(10)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Terms and conditions (Arabic) must be at least 10 characters")
            .MaximumLength(5000)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Terms and conditions (Arabic) must not exceed 5000 characters");

        RuleFor(x => x.TermsAndCondtionsEn)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(10)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Terms and conditions (English) must be at least 10 characters")
            .MaximumLength(5000)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Terms and conditions (English) must not exceed 5000 characters");

        RuleFor(x => x.CancelWarrantyDocumentAr)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(10)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Cancel warranty document (Arabic) must be at least 10 characters")
            .MaximumLength(5000)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Cancel warranty document (Arabic) must not exceed 5000 characters");

        RuleFor(x => x.CancelWarrantyDocumentEn)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(10)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Cancel warranty document (English) must be at least 10 characters")
            .MaximumLength(5000)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Cancel warranty document (English) must not exceed 5000 characters");
    }
}
