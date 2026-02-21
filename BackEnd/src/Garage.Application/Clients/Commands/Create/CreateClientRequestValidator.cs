namespace Garage.Application.Clients.Commands.Create;

using FluentValidation;
using Garage.Contracts.Clients;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Validation.Required")
            .EmailAddress()
                .WithErrorCode("Validation.InvalidEmail")
                .WithMessage("Validation.InvalidEmail");

        RuleFor(x => x.Type)
            .GreaterThan(0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Validation.InvalidFormat");

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

        RuleFor(x => x.CommercialRegister)
            .MaximumLength(50)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Commercial register must not exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CommercialRegister));

        RuleFor(x => x.TaxNumber)
            .MaximumLength(50)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Tax number must not exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));

        RuleFor(x => x.StreetName)
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Street name must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.StreetName));

        RuleFor(x => x.CityName)
            .MaximumLength(100)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("City name must not exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CityName));

        RuleFor(x => x.CountryCode)
            .Length(2)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Country code must be 2 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CountryCode));
    }
}
