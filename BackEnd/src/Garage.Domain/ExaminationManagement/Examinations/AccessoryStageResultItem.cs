using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class AccessoryStageResultItem : Entity
{
    public Guid AccessoryPartId { get; private set; }
    public Guid AccessoryIssueId { get; private set; }

    private AccessoryStageResultItem() { }

    internal AccessoryStageResultItem(Guid partId, Guid issueId)
    {
        AccessoryPartId = partId;
        AccessoryIssueId = issueId;
    }
}
