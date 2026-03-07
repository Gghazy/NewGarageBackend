using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.Bookings.Entities;

public sealed class Booking : AggregateRoot
{
    public Guid ClientId { get; private set; }
    public string ClientNameAr { get; private set; } = default!;
    public string ClientNameEn { get; private set; } = default!;
    public string ClientPhone { get; private set; } = default!;

    public Guid BranchId { get; private set; }
    public string BranchNameAr { get; private set; } = default!;
    public string BranchNameEn { get; private set; } = default!;

    public Guid ManufacturerId { get; private set; }
    public string ManufacturerNameAr { get; private set; } = default!;
    public string ManufacturerNameEn { get; private set; } = default!;

    public Guid CarMarkId { get; private set; }
    public string CarMarkNameAr { get; private set; } = default!;
    public string CarMarkNameEn { get; private set; } = default!;

    public int? Year { get; private set; }
    public string? Transmission { get; private set; }

    public DateOnly ExaminationDate { get; private set; }
    public TimeOnly ExaminationTime { get; private set; }

    public string? Location { get; private set; }
    public string? Notes { get; private set; }

    public BookingStatus Status { get; private set; }
    public Guid? ConvertedExaminationId { get; private set; }

    private Booking() { }

    public static Booking Create(
        Guid clientId, string clientNameAr, string clientNameEn, string clientPhone,
        Guid branchId, string branchNameAr, string branchNameEn,
        Guid manufacturerId, string manufacturerNameAr, string manufacturerNameEn,
        Guid carMarkId, string carMarkNameAr, string carMarkNameEn,
        int? year, string? transmission,
        DateOnly examinationDate, TimeOnly examinationTime,
        string? location, string? notes)
    {
        if (clientId == Guid.Empty) throw new DomainException("Client is required.");
        if (branchId == Guid.Empty) throw new DomainException("Branch is required.");
        if (manufacturerId == Guid.Empty) throw new DomainException("Manufacturer is required.");
        if (carMarkId == Guid.Empty) throw new DomainException("Car mark is required.");

        return new Booking
        {
            ClientId = clientId,
            ClientNameAr = clientNameAr,
            ClientNameEn = clientNameEn,
            ClientPhone = clientPhone,
            BranchId = branchId,
            BranchNameAr = branchNameAr,
            BranchNameEn = branchNameEn,
            ManufacturerId = manufacturerId,
            ManufacturerNameAr = manufacturerNameAr,
            ManufacturerNameEn = manufacturerNameEn,
            CarMarkId = carMarkId,
            CarMarkNameAr = carMarkNameAr,
            CarMarkNameEn = carMarkNameEn,
            Year = year,
            Transmission = transmission?.Trim(),
            ExaminationDate = examinationDate,
            ExaminationTime = examinationTime,
            Location = location?.Trim(),
            Notes = notes?.Trim(),
            Status = BookingStatus.Pending
        };
    }

    public void Update(
        Guid clientId, string clientNameAr, string clientNameEn, string clientPhone,
        Guid branchId, string branchNameAr, string branchNameEn,
        Guid manufacturerId, string manufacturerNameAr, string manufacturerNameEn,
        Guid carMarkId, string carMarkNameAr, string carMarkNameEn,
        int? year, string? transmission,
        DateOnly examinationDate, TimeOnly examinationTime,
        string? location, string? notes)
    {
        EnsureEditable();

        if (clientId == Guid.Empty) throw new DomainException("Client is required.");
        if (branchId == Guid.Empty) throw new DomainException("Branch is required.");
        if (manufacturerId == Guid.Empty) throw new DomainException("Manufacturer is required.");
        if (carMarkId == Guid.Empty) throw new DomainException("Car mark is required.");

        ClientId = clientId;
        ClientNameAr = clientNameAr;
        ClientNameEn = clientNameEn;
        ClientPhone = clientPhone;
        BranchId = branchId;
        BranchNameAr = branchNameAr;
        BranchNameEn = branchNameEn;
        ManufacturerId = manufacturerId;
        ManufacturerNameAr = manufacturerNameAr;
        ManufacturerNameEn = manufacturerNameEn;
        CarMarkId = carMarkId;
        CarMarkNameAr = carMarkNameAr;
        CarMarkNameEn = carMarkNameEn;
        Year = year;
        Transmission = transmission?.Trim();
        ExaminationDate = examinationDate;
        ExaminationTime = examinationTime;
        Location = location?.Trim();
        Notes = notes?.Trim();
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("Only pending bookings can be confirmed.");
        Status = BookingStatus.Confirmed;
    }

    public void MarkConverted(Guid examinationId)
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
            throw new DomainException("Only pending or confirmed bookings can be converted.");
        Status = BookingStatus.Converted;
        ConvertedExaminationId = examinationId;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Converted)
            throw new DomainException("Converted bookings cannot be cancelled.");
        Status = BookingStatus.Cancelled;
    }

    private void EnsureEditable()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
            throw new DomainException("Booking can only be updated in Pending or Confirmed status.");
    }
}
