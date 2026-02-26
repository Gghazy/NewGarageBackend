using FluentValidation;
using Garage.Contracts.Examinations;

namespace Garage.Application.Examinations.Commands.Update;

public sealed class UpdateExaminationRequestValidator : AbstractValidator<UpdateExaminationRequest>
{
    public UpdateExaminationRequestValidator()
    {
        ExaminationRequestRules.AddStartRequiredRules(this);
        ExaminationRequestRules.AddFormatRules(this);
        ExaminationRequestRules.AddItemRules(this);
    }
}
