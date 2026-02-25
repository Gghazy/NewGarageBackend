using FluentValidation;
using Garage.Contracts.Examinations;
using Garage.Domain.ExaminationManagement.Vehicles;

namespace Garage.Application.Examinations.Commands.Create;

public sealed class CreateExaminationRequestValidator : AbstractValidator<CreateExaminationRequest>
{
    private static readonly string[] ValidClientTypes =
        ["Individual", "Company", "Government"];

    private static readonly string[] ValidTypes =
        ["Regular", "Warranty", "PrePurchase"];

    private static readonly string[] ValidMileageUnits =
        [nameof(MileageUnit.Km), nameof(MileageUnit.Mile)];

    private static readonly string[] ValidTransmissions =
        [nameof(TransmissionType.Automatic), nameof(TransmissionType.Manual),
         nameof(TransmissionType.CVT),       nameof(TransmissionType.SemiAutomatic)];

    public CreateExaminationRequestValidator()
    {
        // ── Client common ─────────────────────────────────────────────────────
        RuleFor(x => x.ClientType)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Client type is required.")
            .Must(t => ValidClientTypes.Contains(t))
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage($"Client type must be one of: {string.Join(", ", ValidClientTypes)}.");

        RuleFor(x => x.ClientNameAr)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Client name (Arabic) is required.")
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Client name (Arabic) must not exceed 200 characters.");

        RuleFor(x => x.ClientNameEn)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Client name (English) is required.")
            .MaximumLength(200)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Client name (English) must not exceed 200 characters.");

        RuleFor(x => x.ClientPhone)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Client phone is required.")
            .MaximumLength(20)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Phone must not exceed 20 characters.");

        RuleFor(x => x.ClientEmail)
            .EmailAddress()
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Client email is not valid.")
            .When(x => !string.IsNullOrWhiteSpace(x.ClientEmail));

        // ── Company-specific ──────────────────────────────────────────────────
        RuleFor(x => x.CommercialRegister)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Commercial register is required for company clients.")
            .When(x => x.ClientType == "Company");

        RuleFor(x => x.TaxNumber)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Tax number is required for company clients.")
            .When(x => x.ClientType == "Company");

        RuleFor(x => x.StreetName)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Street name is required for company clients.")
            .When(x => x.ClientType == "Company");

        RuleFor(x => x.CityName)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("City is required for company clients.")
            .When(x => x.ClientType == "Company");

        RuleFor(x => x.CountryCode)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Country code is required for company clients.")
            .MaximumLength(10)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Country code must not exceed 10 characters.")
            .When(x => x.ClientType == "Company");

        // ── Branch ────────────────────────────────────────────────────────────
        RuleFor(x => x.BranchId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Branch is required.");

        // ── Type ──────────────────────────────────────────────────────────────
        RuleFor(x => x.Type)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Examination type is required.")
            .Must(t => ValidTypes.Contains(t))
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.");

        // ── Vehicle ───────────────────────────────────────────────────────────
        RuleFor(x => x.ManufacturerId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Manufacturer is required.");

        RuleFor(x => x.CarMarkId)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Car mark is required.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage($"Year must be between 1900 and {DateTime.UtcNow.Year + 1}.")
            .When(x => x.Year.HasValue);

        RuleFor(x => x.PlateLetters)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Plate letters are required when vehicle has a plate.")
            .When(x => x.HasPlate);

        RuleFor(x => x.PlateNumbers)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Plate numbers are required when vehicle has a plate.")
            .When(x => x.HasPlate);

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0)
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage("Mileage cannot be negative.")
            .When(x => x.Mileage.HasValue);

        RuleFor(x => x.MileageUnit)
            .NotEmpty()
                .WithErrorCode("Validation.Required")
                .WithMessage("Mileage unit is required.")
            .Must(u => ValidMileageUnits.Contains(u))
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage($"Mileage unit must be one of: {string.Join(", ", ValidMileageUnits)}.");

        RuleFor(x => x.Transmission)
            .Must(t => ValidTransmissions.Contains(t))
                .WithErrorCode("Validation.InvalidFormat")
                .WithMessage($"Transmission must be one of: {string.Join(", ", ValidTransmissions)}.")
            .When(x => !string.IsNullOrWhiteSpace(x.Transmission));

        // ── Items (optional for Draft — required before Start) ───────────────
        When(x => x.Items != null && x.Items.Count > 0, () =>
        {
            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ServiceId)
                        .NotEmpty()
                            .WithErrorCode("Validation.Required")
                            .WithMessage("Service ID is required.");

                    item.When(i => i.OverridePrice.HasValue, () =>
                    {
                        item.RuleFor(i => i.OverridePrice!.Value)
                            .GreaterThanOrEqualTo(0)
                                .WithErrorCode("Validation.InvalidFormat")
                                .WithMessage("Override price cannot be negative.");
                    });
                });
        });
    }
}
