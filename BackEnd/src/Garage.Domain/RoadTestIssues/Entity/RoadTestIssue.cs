using Garage.Domain.Common.Primitives;

namespace Garage.Domain.RoadTestIssues.Entity;

public class RoadTestIssue : AggregateRoot
{
    public string NameAr { get; private set; }
    public string NameEn { get; private set; }
    public Guid RoadTestIssueTypeId { get; private set; }

    public virtual RoadTestIssueType RoadTestIssueType { get; private set; }

    private RoadTestIssue() { }

    public RoadTestIssue(string nameAr, string nameEn, Guid roadTestIssueTypeId)
    {
        RoadTestIssueTypeId = roadTestIssueTypeId;
        NameAr = nameAr;
        NameEn = nameEn;
    }

    public void Update(string nameAr, string nameEn, Guid roadTestIssueTypeId)
    {
        RoadTestIssueTypeId = roadTestIssueTypeId;
        NameAr = nameAr;
        NameEn = nameEn;
    }
}
