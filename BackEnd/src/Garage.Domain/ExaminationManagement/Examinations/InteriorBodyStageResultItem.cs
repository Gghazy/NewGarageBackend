using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class InteriorBodyStageResultItem : Entity
{
    public Guid InteriorBodyPartId { get; private set; }
    public Guid InteriorBodyIssueId { get; private set; }

    private InteriorBodyStageResultItem() { }

    internal InteriorBodyStageResultItem(Guid partId, Guid issueId)
    {
        InteriorBodyPartId = partId;
        InteriorBodyIssueId = issueId;
    }
}
