namespace Garage.Application.Services.Commands.Create;

using FluentValidation;
using Garage.Contracts.Services;

public class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequest>
{
    public CreateServiceRequestValidator()
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

        RuleFor(x => x.Stages)
            .NotNull()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .Must(s => s != null && s.Count > 0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("At least one stage must be selected");
    }
}
