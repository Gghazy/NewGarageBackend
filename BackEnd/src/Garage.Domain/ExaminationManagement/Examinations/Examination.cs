using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;

namespace Domain.ExaminationManagement.Examinations;

public sealed class Examination : AggregateRoot
{
    public ClientReference Client { get; private set; } = default!;
    public BranchReference Branch { get; private set; } = default!;
    public VehicleSnapshot Vehicle { get; private set; } = default!;

    public ExaminationType Type { get; private set; }
    public ExaminationStatus Status { get; private set; }

    public bool HasWarranty { get; private set; }
    public bool HasPhotos   { get; private set; }
    public string? MarketerCode { get; private set; }
    public string? Notes { get; private set; }

    public Money TotalPrice { get; private set; } = Money.Zero("EGP");

    private readonly List<ExaminationItem> _items = new();
    public IReadOnlyCollection<ExaminationItem> Items => _items.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Examination() { }

    private Examination(
        ClientReference client,
        BranchReference branch,
        VehicleSnapshot vehicle,
        ExaminationType type,
        bool hasWarranty,
        bool hasPhotos,
        string? marketerCode,
        string currency)
    {
        Client = client;
        Branch = branch;
        Vehicle = vehicle;

        Type        = type;
        HasWarranty = hasWarranty;
        HasPhotos   = hasPhotos;
        MarketerCode = Normalize(marketerCode);

        Status     = ExaminationStatus.Draft;
        TotalPrice = Money.Zero(currency);
    }

    public static Examination Create(
        ClientReference client,
        BranchReference branch,
        VehicleSnapshot vehicle,
        ExaminationType type,
        bool hasWarranty,
        bool hasPhotos,
        string? marketerCode = null,
        string currency = "EGP")
    {
        if (client.ClientId == Guid.Empty)  throw new DomainException("Client is required.");
        if (branch.BranchId == Guid.Empty)  throw new DomainException("Branch is required.");
        if (vehicle.VehicleId == Guid.Empty) throw new DomainException("Vehicle is required.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");

        return new Examination(client, branch, vehicle, type, hasWarranty, hasPhotos, marketerCode, currency);
    }

    public void SetNotes(string? notes) => Notes = Normalize(notes);

    public void Update(bool hasWarranty, bool hasPhotos, string? marketerCode, string? notes)
    {
        EnsureEditable();
        HasWarranty  = hasWarranty;
        HasPhotos    = hasPhotos;
        MarketerCode = Normalize(marketerCode);
        Notes        = Normalize(notes);
    }

    public void UpdateClientSnapshot(ClientReference clientRef)
    {
        EnsureEditable();
        if (clientRef.ClientId == Guid.Empty) throw new DomainException("Client is required.");
        Client = clientRef;
    }

    public void UpdateVehicleSnapshot(VehicleSnapshot snapshot)
    {
        EnsureEditable();
        if (snapshot.VehicleId == Guid.Empty) throw new DomainException("Vehicle is required.");
        Vehicle = snapshot;
    }

    public void AddPayment(Money amount, PaymentMethod method, string? notes)
    {
        if (Status == ExaminationStatus.Cancelled)
            throw new DomainException("Cannot add payment to a cancelled examination.");

        _payments.Add(new Payment(amount, method, notes));
    }

    public void AddItem(ServiceSnapshot service, Money? overridePrice = null)
    {
        EnsureDraft();

        if (service.ServiceId == Guid.Empty) throw new DomainException("Service is required.");
        if (_items.Any(x => x.Service.ServiceId == service.ServiceId))
            throw new DomainException("Service already added.");

        var price = overridePrice ?? service.DefaultPrice;
        _items.Add(new ExaminationItem(service, price));

        RecalculateTotal();
    }

    public void RemoveItem(Guid serviceId)
    {
        EnsureDraft();

        var item = _items.FirstOrDefault(x => x.Service.ServiceId == serviceId);
        if (item is null) return;

        _items.Remove(item);
        RecalculateTotal();
    }

    public void UpdateItemPrice(Guid serviceId, Money newPrice)
    {
        EnsureDraft();

        var item = _items.FirstOrDefault(x => x.Service.ServiceId == serviceId)
                   ?? throw new DomainException("Item not found.");

        item.UpdatePrice(newPrice);
        RecalculateTotal();
    }

    public void Start()
    {
        EnsureDraft();
        if (_items.Count == 0)
            throw new DomainException("Cannot start examination without items.");

        Status = ExaminationStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != ExaminationStatus.InProgress)
            throw new DomainException("Only InProgress examination can be completed.");

        Status = ExaminationStatus.Completed;
    }

    public void Deliver()
    {
        if (Status != ExaminationStatus.Completed)
            throw new DomainException("Only Completed examination can be delivered.");

        Status = ExaminationStatus.Delivered;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == ExaminationStatus.Delivered)
            throw new DomainException("Delivered examination cannot be cancelled.");

        Status = ExaminationStatus.Cancelled;
        Notes = Normalize(reason);
    }

    private void EnsureEditable()
    {
        if (Status != ExaminationStatus.Draft && Status != ExaminationStatus.InProgress)
            throw new DomainException("Examination can only be updated in Draft or InProgress status.");
    }

    private void EnsureDraft()
    {
        if (Status != ExaminationStatus.Draft)
            throw new DomainException("You can modify examination only in Draft status.");
    }

    private void RecalculateTotal()
    {
        if (_items.Count == 0)
        {
            TotalPrice = Money.Zero(TotalPrice.Currency);
            return;
        }

        var total = Money.Zero(_items[0].Price.Currency);
        foreach (var i in _items)
            total = total.Add(i.Price);

        TotalPrice = total;
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}