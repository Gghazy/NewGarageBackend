using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class MechanicalStageResultIssueItem : Entity
{
    public Guid MechPartId { get; private set; }
    public Guid MechIssueId { get; private set; }

    private MechanicalStageResultIssueItem() { }

    internal MechanicalStageResultIssueItem(Guid partId, Guid issueId)
    {
        MechPartId = partId;
        MechIssueId = issueId;
    }
}
