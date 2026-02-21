namespace Garage.Application.Employees.Commands.Create;

using FluentValidation;
using Garage.Contracts.Employees;

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");

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

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Validation.InvalidFormat");

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .EmailAddress()
                .WithErrorCode("Validation.InvalidEmail")
                .WithMessage("Validation.InvalidEmail");

        RuleFor(x => x.BranchId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");

        RuleFor(x => x.RoleId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");
    }
}
