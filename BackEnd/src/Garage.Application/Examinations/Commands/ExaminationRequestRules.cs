using FluentValidation;
using Garage.Contracts.Examinations;
using Garage.Domain.ExaminationManagement.Vehicles;

namespace Garage.Application.Examinations.Commands;

public static class ExaminationRequestRules
{
    public static readonly string[] ValidClientTypes =
        ["Individual", "Company", "Government"];

    public static readonly string[] ValidTypes =
        ["Regular", "Warranty", "PrePurchase"];

    public static readonly string[] ValidMileageUnits =
        [nameof(MileageUnit.Km), nameof(MileageUnit.Mile)];

    public static readonly string[] ValidTransmissions =
        [nameof(TransmissionType.Automatic), nameof(TransmissionType.Manual),
         nameof(TransmissionType.CVT),       nameof(TransmissionType.SemiAutomatic)];

    /// <summary>Rules enforced only when StartAfterSave (non-draft).</summary>
    public static void AddStartRequiredRules<T>(AbstractValidator<T> v) where T : IExaminationRequest
    {
        v.When(x => x.StartAfterSave, () =>
        {
            // Client
            v.RuleFor(x => x.ClientType).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Client type is required.");
            v.RuleFor(x => x.ClientNameAr).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Client name (Arabic) is required.");
            v.RuleFor(x => x.ClientNameEn).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Client name (English) is required.");
            v.RuleFor(x => x.ClientPhone).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Client phone is required.");

            // Company-specific
            v.RuleFor(x => x.CommercialRegister).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Commercial register is required for company clients.").When(x => x.ClientType == "Company");
            v.RuleFor(x => x.TaxNumber).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Tax number is required for company clients.").When(x => x.ClientType == "Company");
            v.RuleFor(x => x.StreetName).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Street name is required for company clients.").When(x => x.ClientType == "Company");
            v.RuleFor(x => x.CityName).NotEmpty().WithErrorCode("Validation.Required").WithMessage("City is required for company clients.").When(x => x.ClientType == "Company");
            v.RuleFor(x => x.CountryCode).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Country code is required for company clients.").When(x => x.ClientType == "Company");

            // Branch, Type
            v.RuleFor(x => x.BranchId).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Branch is required.");
            v.RuleFor(x => x.Type).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Examination type is required.");

            // Vehicle
            v.RuleFor(x => x.ManufacturerId).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Manufacturer is required.");
            v.RuleFor(x => x.CarMarkId).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Car mark is required.");
            v.RuleFor(x => x.Year).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Year is required.");
            v.RuleFor(x => x.Color).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Color is required.");
            v.RuleFor(x => x.Transmission).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Transmission is required.");
            v.RuleFor(x => x.Mileage).NotNull().WithErrorCode("Validation.Required").WithMessage("Mileage is required.");
            v.RuleFor(x => x.MileageUnit).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Mileage unit is required.");

            // Plate vs VIN
            v.RuleFor(x => x.PlateLetters).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Plate letters are required when vehicle has a plate.").When(x => x.HasPlate);
            v.RuleFor(x => x.PlateNumbers).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Plate numbers are required when vehicle has a plate.").When(x => x.HasPlate);
            v.RuleFor(x => x.Vin).NotEmpty().WithErrorCode("Validation.Required").WithMessage("VIN is required when vehicle has no plate.").When(x => !x.HasPlate);

            // Items
            v.RuleFor(x => x.Items).NotEmpty().WithErrorCode("Validation.Required").WithMessage("At least one service item is required.");
        });
    }

    /// <summary>Format rules — always enforced (draft and start).</summary>
    public static void AddFormatRules<T>(AbstractValidator<T> v) where T : IExaminationRequest
    {
        v.RuleFor(x => x.ClientType).Must(t => ValidClientTypes.Contains(t)).WithErrorCode("Validation.InvalidFormat").WithMessage($"Client type must be one of: {string.Join(", ", ValidClientTypes)}.").When(x => !string.IsNullOrWhiteSpace(x.ClientType));
        v.RuleFor(x => x.ClientNameAr).MaximumLength(200).WithErrorCode("Validation.InvalidFormat").WithMessage("Client name (Arabic) must not exceed 200 characters.").When(x => !string.IsNullOrWhiteSpace(x.ClientNameAr));
        v.RuleFor(x => x.ClientNameEn).MaximumLength(200).WithErrorCode("Validation.InvalidFormat").WithMessage("Client name (English) must not exceed 200 characters.").When(x => !string.IsNullOrWhiteSpace(x.ClientNameEn));
        v.RuleFor(x => x.ClientPhone).MaximumLength(20).WithErrorCode("Validation.InvalidFormat").WithMessage("Phone must not exceed 20 characters.").When(x => !string.IsNullOrWhiteSpace(x.ClientPhone));
        v.RuleFor(x => x.ClientEmail).EmailAddress().WithErrorCode("Validation.InvalidFormat").WithMessage("Client email is not valid.").When(x => !string.IsNullOrWhiteSpace(x.ClientEmail));
        v.RuleFor(x => x.CountryCode).MaximumLength(10).WithErrorCode("Validation.InvalidFormat").WithMessage("Country code must not exceed 10 characters.").When(x => !string.IsNullOrWhiteSpace(x.CountryCode));
        v.RuleFor(x => x.Type).Must(t => ValidTypes.Contains(t)).WithErrorCode("Validation.InvalidFormat").WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.").When(x => !string.IsNullOrWhiteSpace(x.Type));
        v.RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.UtcNow.Year + 1).WithErrorCode("Validation.InvalidFormat").WithMessage($"Year must be between 1900 and {DateTime.UtcNow.Year + 1}.").When(x => x.Year.HasValue);
        v.RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0).WithErrorCode("Validation.InvalidFormat").WithMessage("Mileage cannot be negative.").When(x => x.Mileage.HasValue);
        v.RuleFor(x => x.MileageUnit).Must(u => ValidMileageUnits.Contains(u)).WithErrorCode("Validation.InvalidFormat").WithMessage($"Mileage unit must be one of: {string.Join(", ", ValidMileageUnits)}.").When(x => !string.IsNullOrWhiteSpace(x.MileageUnit));
        v.RuleFor(x => x.Transmission).Must(t => ValidTransmissions.Contains(t)).WithErrorCode("Validation.InvalidFormat").WithMessage($"Transmission must be one of: {string.Join(", ", ValidTransmissions)}.").When(x => !string.IsNullOrWhiteSpace(x.Transmission));
    }

    /// <summary>Item format rules — always enforced when items are provided.</summary>
    public static void AddItemRules<T>(AbstractValidator<T> v) where T : IExaminationRequest
    {
        v.When(x => x.Items != null && x.Items.Count > 0, () =>
        {
            v.RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ServiceId).NotEmpty().WithErrorCode("Validation.Required").WithMessage("Service ID is required.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithErrorCode("Validation.InvalidFormat").WithMessage("Quantity must be greater than zero.");
            });
        });
    }
}
