namespace Garage.Application.Roles.Commands.UpsertRole;

using FluentValidation;
using Garage.Contracts.Roles;

public class UpsertRoleRequestValidator : AbstractValidator<UpsertRoleRequest>
{
    public UpsertRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Role name must be at least 2 characters")
            .MaximumLength(100)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Role name must not exceed 100 characters");

        RuleFor(x => x.Permissions)
            .NotNull()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .Must(p => p != null && p.Count > 0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("At least one permission must be selected");
    }
}
