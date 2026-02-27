using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class SensorStageResultItem : Entity
{
    public Guid SensorIssueId { get; private set; }
    public string Evaluation { get; private set; } = default!;

    private SensorStageResultItem() { }

    internal SensorStageResultItem(Guid sensorIssueId, string evaluation)
    {
        SensorIssueId = sensorIssueId;
        Evaluation = evaluation;
    }
}
