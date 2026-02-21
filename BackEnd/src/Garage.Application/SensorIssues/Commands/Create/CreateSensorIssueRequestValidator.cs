namespace Garage.Application.SensorIssues.Commands.Create;

using FluentValidation;
using Garage.Contracts.SensorIssues;

public class CreateSensorIssueRequestValidator : AbstractValidator<CreateSensorIssueRequest>
{
    public CreateSensorIssueRequestValidator()
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

        RuleFor(x => x.code)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MaximumLength(50)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Code must not exceed 50 characters");
    }
}
