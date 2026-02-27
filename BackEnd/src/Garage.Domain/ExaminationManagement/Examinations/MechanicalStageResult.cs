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

    internal void AddItem(Guid issueTypeId, Guid issueId)
    {
        if (issueTypeId == Guid.Empty)
            throw new DomainException("Issue type is required.");
        if (issueId == Guid.Empty)
            throw new DomainException("Issue is required.");

        _items.Add(new MechanicalStageResultItem(issueTypeId, issueId));
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
