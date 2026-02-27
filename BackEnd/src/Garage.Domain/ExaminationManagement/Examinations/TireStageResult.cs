using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class TireStageResult : Entity
{
    public Guid ExaminationId { get; private set; }
    public bool NoIssuesFound { get; private set; }
    public string? Comments { get; private set; }

    private readonly List<TireStageResultItem> _items = new();
    public IReadOnlyCollection<TireStageResultItem> Items => _items.AsReadOnly();

    private TireStageResult() { }

    internal static TireStageResult Create(
        Guid examinationId,
        bool noIssuesFound,
        string? comments)
    {
        if (examinationId == Guid.Empty)
            throw new DomainException("Examination is required.");

        return new TireStageResult
        {
            ExaminationId = examinationId,
            NoIssuesFound = noIssuesFound,
            Comments = Normalize(comments)
        };
    }

    internal void AddItem(string position, int? year, int? week, string? condition)
    {
        if (NoIssuesFound)
            throw new DomainException("Cannot add items when marked as no issues found.");

        if (string.IsNullOrWhiteSpace(position))
            throw new DomainException("Tire position is required.");

        if (_items.Any(x => x.Position == position))
            throw new DomainException($"Tire position '{position}' already added.");

        _items.Add(new TireStageResultItem(position, year, week, condition));
    }

    internal void Update(bool noIssuesFound, string? comments)
    {
        NoIssuesFound = noIssuesFound;
        Comments = Normalize(comments);
    }

    internal void ClearItems() => _items.Clear();

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
