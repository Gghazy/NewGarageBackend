using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class DashboardIndicatorsStageResultItem : Entity
{
    public string Key { get; private set; } = default!;
    public decimal? Value { get; private set; }
    public bool NotApplicable { get; private set; }

    private DashboardIndicatorsStageResultItem() { }

    internal DashboardIndicatorsStageResultItem(string key, decimal? value, bool notApplicable)
    {
        Key = key;
        Value = notApplicable ? null : value;
        NotApplicable = notApplicable;
    }
}
