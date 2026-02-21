namespace Garage.Application.Branches.Commands.Update;

using FluentValidation;
using Garage.Contracts.Branches;

public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
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
    }
}
