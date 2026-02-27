using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class RoadTestStageResultItem : Entity
{
    public Guid RoadTestIssueTypeId { get; private set; }
    public Guid RoadTestIssueId { get; private set; }

    private RoadTestStageResultItem() { }

    internal RoadTestStageResultItem(Guid issueTypeId, Guid issueId)
    {
        RoadTestIssueTypeId = issueTypeId;
        RoadTestIssueId = issueId;
    }
}
