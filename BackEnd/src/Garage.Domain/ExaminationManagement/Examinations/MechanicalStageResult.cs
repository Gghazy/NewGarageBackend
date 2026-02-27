using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class MechanicalStageResult : Entity
{
    public Guid ExaminationId { get; private set; }
    public bool NoIssuesFound { get; private set; }
    public string? Comments { get; private set; }

    private readonly List<MechanicalStageResultItem> _items = new();
    public IReadOnlyCollection<MechanicalStageResultItem> Items => _items.AsReadOnly();

    private readonly List<MechanicalStageResultIssueItem> _issueItems = new();
    public IReadOnlyCollection<MechanicalStageResultIssueItem> IssueItems => _issueItems.AsReadOnly();

    private MechanicalStageResult() { }

    internal static MechanicalStageResult Create(
        Guid examinationId,
        bool noIssuesFound,
        string? comments)
    {
        if (examinationId == Guid.Empty)
            throw new DomainException("Examination is required.");

        return new MechanicalStageResult
        {
            ExaminationId = examinationId,
            NoIssuesFound = noIssuesFound,
            Comments = Normalize(comments)
        };
    }

    internal void AddItem(Guid partTypeId, Guid partId)
    {
        if (partTypeId == Guid.Empty)
            throw new DomainException("Part type is required.");
        if (partId == Guid.Empty)
            throw new DomainException("Part is required.");

        _items.Add(new MechanicalStageResultItem(partTypeId, partId));
    }

    internal void Update(bool noIssuesFound, string? comments)
    {
        NoIssuesFound = noIssuesFound;
        Comments = Normalize(comments);
    }

    internal void AddIssueItem(Guid partId, Guid issueId)
    {
        if (partId == Guid.Empty)
            throw new DomainException("Part is required.");
        if (issueId == Guid.Empty)
            throw new DomainException("Issue is required.");

        _issueItems.Add(new MechanicalStageResultIssueItem(partId, issueId));
    }

    internal void ClearItems() => _items.Clear();

    internal void ClearIssueItems() => _issueItems.Clear();

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
