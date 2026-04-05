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

    private readonly List<BookingHistory> _history = new();
    public IReadOnlyCollection<BookingHistory> History => _history.AsReadOnly();

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

        var booking = new Booking
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
        booking.AddHistory(BookingHistoryAction.Created);
        return booking;
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
        AddHistory(BookingHistoryAction.Updated);
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("Only pending bookings can be confirmed.");
        Status = BookingStatus.Confirmed;
        AddHistory(BookingHistoryAction.Confirmed);
    }

    public void MarkConverted(Guid examinationId)
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException("Only confirmed bookings can be converted.");
        Status = BookingStatus.Converted;
        ConvertedExaminationId = examinationId;
        AddHistory(BookingHistoryAction.Converted, $"ExaminationId: {examinationId}");
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
            throw new DomainException("Only pending or confirmed bookings can be cancelled.");
        Status = BookingStatus.Cancelled;
        AddHistory(BookingHistoryAction.Cancelled);
    }

    public void MarkDeleted()
    {
        AddHistory(BookingHistoryAction.Deleted);
    }

    private void AddHistory(BookingHistoryAction action, string? details = null)
    {
        _history.Add(new BookingHistory(Id, action, details));
    }

    private void EnsureEditable()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
            throw new DomainException("Booking can only be updated in Pending or Confirmed status.");
    }
}
