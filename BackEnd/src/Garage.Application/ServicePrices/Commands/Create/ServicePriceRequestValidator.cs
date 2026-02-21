namespace Garage.Application.ServicePrices.Commands.Create;

using FluentValidation;
using Garage.Contracts.ServicePrices;

public class ServicePriceRequestValidator : AbstractValidator<ServicePriceRequest>
{
    public ServicePriceRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");

        RuleFor(x => x.MarkId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required");

        RuleFor(x => x.FromYear)
            .GreaterThan(1900)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("From year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("From year cannot be in the future");

        RuleFor(x => x.ToYear)
            .GreaterThan(1900)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("To year must be greater than 1900")
            .GreaterThanOrEqualTo(x => x.FromYear)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("To year must be greater than or equal to From year");

        RuleFor(x => x.Price)
            .GreaterThan(0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Price must be greater than 0");
    }
}
