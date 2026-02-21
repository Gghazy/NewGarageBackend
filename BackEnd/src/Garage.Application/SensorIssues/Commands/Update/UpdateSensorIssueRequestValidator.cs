namespace Garage.Application.SensorIssues.Commands.Update;

using FluentValidation;
using Garage.Contracts.SensorIssues;

public class UpdateSensorIssueRequestValidator : AbstractValidator<UpdateSensorIssueRequest>
{
    public UpdateSensorIssueRequestValidator()
    {
        RuleFor(x => x.NameAr)
            .MinimumLength(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (Arabic) must be at least 2 characters")
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (Arabic) must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));

        RuleFor(x => x.NameEn)
            .MinimumLength(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (English) must be at least 2 characters")
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Name (English) must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NameEn));

        RuleFor(x => x.code)
            .MaximumLength(50)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Code must not exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.code));
    }
}
