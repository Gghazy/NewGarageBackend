using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class TireStageResultItem : Entity
{
    public string Position { get; private set; } = default!;
    public int? Year { get; private set; }
    public int? Week { get; private set; }
    public string? Condition { get; private set; }

    private TireStageResultItem() { }

    internal TireStageResultItem(string position, int? year, int? week, string? condition)
    {
        Position = position;
        Year = year;
        Week = week;
        Condition = condition;
    }
}
