using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class MechanicalStageResultItem : Entity
{
    public Guid MechPartTypeId { get; private set; }
    public Guid MechPartId { get; private set; }

    private MechanicalStageResultItem() { }

    internal MechanicalStageResultItem(Guid partTypeId, Guid partId)
    {
        MechPartTypeId = partTypeId;
        MechPartId = partId;
    }
}
