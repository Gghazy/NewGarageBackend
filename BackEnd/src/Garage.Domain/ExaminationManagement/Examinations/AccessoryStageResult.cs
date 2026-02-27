using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class AccessoryStageResult : Entity
{
    public Guid ExaminationId { get; private set; }
    public bool NoIssuesFound { get; private set; }
    public string? Comments { get; private set; }

    private readonly List<AccessoryStageResultItem> _items = new();
    public IReadOnlyCollection<AccessoryStageResultItem> Items => _items.AsReadOnly();

    private AccessoryStageResult() { }

    internal static AccessoryStageResult Create(
        Guid examinationId,
        bool noIssuesFound,
        string? comments)
    {
        if (examinationId == Guid.Empty)
            throw new DomainException("Examination is required.");

        return new AccessoryStageResult
        {
            ExaminationId = examinationId,
            NoIssuesFound = noIssuesFound,
            Comments = Normalize(comments)
        };
    }

    internal void AddItem(Guid partId, Guid issueId)
    {
        if (partId == Guid.Empty)
            throw new DomainException("Accessory part is required.");
        if (issueId == Guid.Empty)
            throw new DomainException("Accessory issue is required.");

        _items.Add(new AccessoryStageResultItem(partId, issueId));
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
