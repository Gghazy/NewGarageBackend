using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class DashboardIndicatorsStageResult : Entity
{
    public Guid ExaminationId { get; private set; }
    public string? Comments { get; private set; }

    private readonly List<DashboardIndicatorsStageResultItem> _items = new();
    public IReadOnlyCollection<DashboardIndicatorsStageResultItem> Items => _items.AsReadOnly();

    private DashboardIndicatorsStageResult() { }

    internal static DashboardIndicatorsStageResult Create(Guid examinationId, string? comments)
    {
        if (examinationId == Guid.Empty)
            throw new DomainException("Examination is required.");

        return new DashboardIndicatorsStageResult
        {
            ExaminationId = examinationId,
            Comments = Normalize(comments)
        };
    }

    internal void AddIndicator(string key, decimal? value, bool notApplicable)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Indicator key is required.");

        if (_items.Any(x => x.Key == key))
            throw new DomainException($"Indicator '{key}' already added.");

        _items.Add(new DashboardIndicatorsStageResultItem(key, value, notApplicable));
    }

    internal void Update(string? comments)
    {
        Comments = Normalize(comments);
    }

    internal void ClearItems() => _items.Clear();

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
