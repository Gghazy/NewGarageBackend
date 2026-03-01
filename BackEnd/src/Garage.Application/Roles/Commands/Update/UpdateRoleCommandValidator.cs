using FluentValidation;

namespace Garage.Application.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Role ID is required");

        RuleFor(x => x.Request.RoleName)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .MinimumLength(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Role name must be at least 2 characters")
            .MaximumLength(100)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Role name must not exceed 100 characters");

        RuleFor(x => x.Request.Permissions)
            .NotNull()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .Must(p => p != null && p.Count > 0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("At least one permission must be selected");
    }
}
