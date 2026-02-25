using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.ExaminationManagement.Vehicles;

namespace Garage.Domain.ExaminationManagement.Examinations;

/// <summary>
/// Immutable point-in-time snapshot of vehicle data captured when an Examination is created.
/// </summary>
public sealed record VehicleSnapshot(
    Guid             VehicleId,
    Guid             ManufacturerId,
    string           ManufacturerNameAr,
    string           ManufacturerNameEn,
    Guid             CarMarkId,
    string           CarMarkNameAr,
    string           CarMarkNameEn,
    int?             Year,
    string?          Color,
    string?          Vin,
    bool             HasPlate,
    PlateNumber?     Plate,
    decimal?         Mileage,
    MileageUnit      MileageUnit,
    TransmissionType? Transmission
)
{
    // Parameterless constructor required by EF Core — PlateNumber is an owned type and cannot be bound via constructor parameters
    private VehicleSnapshot()
        : this(Guid.Empty, Guid.Empty, string.Empty, string.Empty, Guid.Empty, string.Empty, string.Empty,
               null, null, null, false, null, null, MileageUnit.Km, null) { }
}
