using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;

namespace Garage.Domain.ExaminationManagement.Vehicles;

public sealed class Vehicle : AggregateRoot
{
    // ── Manufacturer ─────────────────────────────────────────────────────────
    public Guid   ManufacturerId   { get; private set; }
    public string ManufacturerNameAr { get; private set; } = default!;
    public string ManufacturerNameEn { get; private set; } = default!;

    // ── Car mark (model) ─────────────────────────────────────────────────────
    public Guid   CarMarkId      { get; private set; }
    public string CarMarkNameAr  { get; private set; } = default!;
    public string CarMarkNameEn  { get; private set; } = default!;

    // ── Basic info ───────────────────────────────────────────────────────────
    public int?  Year  { get; private set; }
    public string? Color { get; private set; }
    public string? Vin   { get; private set; }

    // ── Plate ────────────────────────────────────────────────────────────────
    public bool         HasPlate { get; private set; }
    public PlateNumber? Plate    { get; private set; }

    // ── Odometer ─────────────────────────────────────────────────────────────
    public decimal?     Mileage     { get; private set; }
    public MileageUnit  MileageUnit { get; private set; } = MileageUnit.Km;

    // ── Transmission ─────────────────────────────────────────────────────────
    public TransmissionType? Transmission { get; private set; }

    private Vehicle() { } // EF Core

    private Vehicle(
        Guid manufacturerId, string manufacturerNameAr, string manufacturerNameEn,
        Guid carMarkId,      string carMarkNameAr,      string carMarkNameEn,
        int? year, string? color, string? vin,
        bool hasPlate, PlateNumber? plate,
        decimal? mileage, MileageUnit mileageUnit,
        TransmissionType? transmission)
    {
        ManufacturerId    = manufacturerId;
        ManufacturerNameAr = manufacturerNameAr;
        ManufacturerNameEn = manufacturerNameEn;
        CarMarkId         = carMarkId;
        CarMarkNameAr     = carMarkNameAr;
        CarMarkNameEn     = carMarkNameEn;
        Year              = year;
        Color             = Normalize(color);
        Vin               = Normalize(vin);
        HasPlate          = hasPlate;
        Plate             = hasPlate ? plate : null;
        Mileage           = mileage;
        MileageUnit       = mileageUnit;
        Transmission      = transmission;
    }

    // ── Factory ──────────────────────────────────────────────────────────────

    public static Vehicle Create(
        Guid manufacturerId, string manufacturerNameAr, string manufacturerNameEn,
        Guid carMarkId,      string carMarkNameAr,      string carMarkNameEn,
        int? year, string? color, string? vin,
        bool hasPlate, PlateNumber? plate,
        decimal? mileage, MileageUnit mileageUnit,
        TransmissionType? transmission)
    {
        Validate(manufacturerId, carMarkId, year, hasPlate, plate, mileage);

        return new Vehicle(
            manufacturerId, manufacturerNameAr, manufacturerNameEn,
            carMarkId,      carMarkNameAr,      carMarkNameEn,
            year, color, vin,
            hasPlate, plate,
            mileage, mileageUnit,
            transmission);
    }

    // ── Behaviour ────────────────────────────────────────────────────────────

    public void Update(
        Guid manufacturerId, string manufacturerNameAr, string manufacturerNameEn,
        Guid carMarkId,      string carMarkNameAr,      string carMarkNameEn,
        int? year, string? color, string? vin,
        bool hasPlate, PlateNumber? plate,
        decimal? mileage, MileageUnit mileageUnit,
        TransmissionType? transmission)
    {
        Validate(manufacturerId, carMarkId, year, hasPlate, plate, mileage);

        ManufacturerId     = manufacturerId;
        ManufacturerNameAr = manufacturerNameAr;
        ManufacturerNameEn = manufacturerNameEn;
        CarMarkId          = carMarkId;
        CarMarkNameAr      = carMarkNameAr;
        CarMarkNameEn      = carMarkNameEn;
        Year               = year;
        Color              = Normalize(color);
        Vin                = Normalize(vin);
        HasPlate           = hasPlate;
        Plate              = hasPlate ? plate : null;
        Mileage            = mileage;
        MileageUnit        = mileageUnit;
        Transmission       = transmission;
    }

    /// <summary>Creates a point-in-time snapshot to embed inside an Examination.</summary>
    public VehicleSnapshot ToSnapshot() =>
        new(
            VehicleId:          Id,
            ManufacturerId:     ManufacturerId,
            ManufacturerNameAr: ManufacturerNameAr,
            ManufacturerNameEn: ManufacturerNameEn,
            CarMarkId:          CarMarkId,
            CarMarkNameAr:      CarMarkNameAr,
            CarMarkNameEn:      CarMarkNameEn,
            Year:               Year,
            Color:              Color,
            Vin:                Vin,
            HasPlate:           HasPlate,
            Plate:              Plate,
            Mileage:            Mileage,
            MileageUnit:        MileageUnit,
            Transmission:       Transmission);

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void Validate(
        Guid manufacturerId, Guid carMarkId,
        int? year, bool hasPlate, PlateNumber? plate, decimal? mileage)
    {
        if (manufacturerId == Guid.Empty)
            throw new DomainException("Manufacturer is required.");

        if (carMarkId == Guid.Empty)
            throw new DomainException("Car mark is required.");

        if (year.HasValue && (year.Value < 1900 || year.Value > DateTime.UtcNow.Year + 1))
            throw new DomainException($"Year must be between 1900 and {DateTime.UtcNow.Year + 1}.");

        if (hasPlate && plate is null)
            throw new DomainException("Plate number is required when the vehicle has a plate.");

        if (mileage.HasValue && mileage.Value < 0)
            throw new DomainException("Mileage cannot be negative.");
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
