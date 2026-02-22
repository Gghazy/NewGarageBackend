using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;

namespace Domain.ExaminationManagement.Examinations;

public sealed class ExaminationItem : Entity
{
    public ServiceSnapshot Service { get; private set; } = default!;
    public Money Price { get; private set; } = Money.Zero();
    public ExaminationItemStatus Status { get; private set; } = ExaminationItemStatus.Pending;
    public string? Notes { get; private set; }

    private ExaminationItem() { } // EF

    internal ExaminationItem(ServiceSnapshot service, Money price)
    {
        Service = service;
        Price = price;
        Status = ExaminationItemStatus.Pending;
    }

    internal void UpdatePrice(Money newPrice) => Price = newPrice;

    public void SetResult(ExaminationItemStatus status, string? notes)
    {
        Status = status;
        Notes = Normalize(notes);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}