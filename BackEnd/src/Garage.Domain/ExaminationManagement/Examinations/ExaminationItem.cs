using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;

namespace Domain.ExaminationManagement.Examinations;

public sealed class ExaminationItem : Entity
{
    public ServiceSnapshot Service { get; private set; } = default!;
    public int Quantity { get; private set; } = 1;
    public decimal? OverridePrice { get; private set; }
    public ExaminationItemStatus Status { get; private set; } = ExaminationItemStatus.Pending;
    public string? Notes { get; private set; }

    private ExaminationItem() { } // EF

    internal ExaminationItem(ServiceSnapshot service, int quantity = 1, decimal? overridePrice = null)
    {
        Service = service;
        Quantity = quantity;
        OverridePrice = overridePrice;
        Status = ExaminationItemStatus.Pending;
    }

    public void SetResult(ExaminationItemStatus status, string? notes)
    {
        Status = status;
        Notes = Normalize(notes);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
