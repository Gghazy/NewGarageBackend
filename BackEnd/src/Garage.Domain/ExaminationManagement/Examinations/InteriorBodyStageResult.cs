using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class InteriorBodyStageResult : Entity
{
    public Guid ExaminationId { get; private set; }
    public string? Comments { get; private set; }
    public bool NoIssuesFound { get; private set; }

    private readonly List<InteriorBodyStageResultItem> _items = new();
    public IReadOnlyCollection<InteriorBodyStageResultItem> Items => _items.AsReadOnly();

    private InteriorBodyStageResult() { }

    internal static InteriorBodyStageResult Create(Guid examinationId, bool noIssuesFound, string? comments)
    {
        if (examinationId == Guid.Empty)
            throw new DomainException("Examination is required.");

        return new InteriorBodyStageResult
        {
            ExaminationId = examinationId,
            NoIssuesFound = noIssuesFound,
            Comments = Normalize(comments)
        };
    }

    internal void AddItem(Guid partId, Guid issueId)
    {
        if (partId == Guid.Empty)
            throw new DomainException("Part is required.");
        if (issueId == Guid.Empty)
            throw new DomainException("Issue is required.");

        _items.Add(new InteriorBodyStageResultItem(partId, issueId));
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
