using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class ExteriorBodyStageResultItem : Entity
{
    public Guid ExteriorBodyPartId { get; private set; }
    public Guid ExteriorBodyIssueId { get; private set; }

    private ExteriorBodyStageResultItem() { }

    internal ExteriorBodyStageResultItem(Guid partId, Guid issueId)
    {
        ExteriorBodyPartId = partId;
        ExteriorBodyIssueId = issueId;
    }
}
