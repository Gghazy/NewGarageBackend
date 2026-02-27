using Garage.Domain.Common.Primitives;

namespace Garage.Domain.RoadTestIssues.Entity;

public class RoadTestIssueType : LookupBase
{
    private readonly List<RoadTestIssue> _roadTestIssues = new();

    public RoadTestIssueType() { }
    public RoadTestIssueType(string nameAr, string nameEn) : base(nameAr, nameEn) { }

    public IReadOnlyCollection<RoadTestIssue> RoadTestIssues => _roadTestIssues;
}
