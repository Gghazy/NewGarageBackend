using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class InteriorDecorStageResultItem : Entity
{
    public Guid InsideAndDecorPartId { get; private set; }
    public Guid InsideAndDecorPartIssueId { get; private set; }

    private InteriorDecorStageResultItem() { }

    internal InteriorDecorStageResultItem(Guid partId, Guid issueId)
    {
        InsideAndDecorPartId = partId;
        InsideAndDecorPartIssueId = issueId;
    }
}
