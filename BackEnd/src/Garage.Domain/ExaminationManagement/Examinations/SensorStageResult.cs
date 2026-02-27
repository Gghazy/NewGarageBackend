using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class SensorStageResult : Entity
{
    public Guid ExaminationId { get; private set; }
    public bool NoIssuesFound { get; private set; }
    public int CylinderCount { get; private set; }
    public string? Comments { get; private set; }

    private readonly List<SensorStageResultItem> _items = new();
    public IReadOnlyCollection<SensorStageResultItem> Items => _items.AsReadOnly();

    private SensorStageResult() { }

    internal static SensorStageResult Create(
        Guid examinationId,
        bool noIssuesFound,
        int cylinderCount,
        string? comments)
    {
        if (examinationId == Guid.Empty)
            throw new DomainException("Examination is required.");

        var result = new SensorStageResult
        {
            ExaminationId = examinationId,
            NoIssuesFound = noIssuesFound,
            CylinderCount = cylinderCount,
            Comments = Normalize(comments)
        };

        return result;
    }

    internal void AddIssue(Guid sensorIssueId, string evaluation)
    {
        if (NoIssuesFound)
            throw new DomainException("Cannot add issues when marked as no issues found.");

        if (sensorIssueId == Guid.Empty)
            throw new DomainException("Sensor issue is required.");

        if (_items.Any(x => x.SensorIssueId == sensorIssueId))
            throw new DomainException("Sensor issue already added.");

        _items.Add(new SensorStageResultItem(sensorIssueId, evaluation));
    }

    internal void Update(bool noIssuesFound, int cylinderCount, string? comments)
    {
        NoIssuesFound = noIssuesFound;
        CylinderCount = cylinderCount;
        Comments = Normalize(comments);
    }

    internal void ClearIssues() => _items.Clear();

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
