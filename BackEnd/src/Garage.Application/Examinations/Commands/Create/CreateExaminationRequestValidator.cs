using FluentValidation;
using Garage.Contracts.Examinations;

namespace Garage.Application.Examinations.Commands.Create;

public sealed class CreateExaminationRequestValidator : AbstractValidator<CreateExaminationRequest>
{
    public CreateExaminationRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Client is required.");

        ExaminationRequestRules.AddStartRequiredRules(this);
        ExaminationRequestRules.AddFormatRules(this);
        ExaminationRequestRules.AddItemRules(this);
    }
}
