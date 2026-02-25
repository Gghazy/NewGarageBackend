using FluentValidation;
using Garage.Contracts.Examinations;

namespace Garage.Application.Examinations.Commands.AddPayment;

public sealed class AddPaymentRequestValidator : AbstractValidator<AddPaymentRequest>
{
    private static readonly string[] ValidMethods =
        ["Cash", "Card", "BankTransfer", "Cheque"];

    public AddPaymentRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Payment amount must be greater than 0.");

        RuleFor(x => x.Method)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Payment method is required.")
            .Must(m => ValidMethods.Contains(m))
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage($"Payment method must be one of: {string.Join(", ", ValidMethods)}.");

        RuleFor(x => x.Currency)
            .MaximumLength(3)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Currency code must not exceed 3 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Currency));
    }
}
