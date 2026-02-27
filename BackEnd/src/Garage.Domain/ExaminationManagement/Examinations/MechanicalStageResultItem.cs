using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class MechanicalStageResultItem : Entity
{
    public Guid MechIssueTypeId { get; private set; }
    public Guid MechIssueId { get; private set; }

    private MechanicalStageResultItem() { }

    internal MechanicalStageResultItem(Guid issueTypeId, Guid issueId)
    {
        MechIssueTypeId = issueTypeId;
        MechIssueId = issueId;
    }
}
