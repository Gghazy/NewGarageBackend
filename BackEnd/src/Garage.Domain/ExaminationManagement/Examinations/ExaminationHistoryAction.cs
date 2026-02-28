namespace Garage.Domain.ExaminationManagement.Examinations;

public enum ExaminationHistoryAction
{
    Created = 1,
    Updated = 2,
    Started = 3,
    BeganWork = 4,
    Completed = 5,
    Delivered = 6,
    Reopened = 7,
    Cancelled = 8,
    Deleted = 9,

    SensorStageSaved = 10,
    DashboardIndicatorsStageSaved = 11,
    InteriorDecorStageSaved = 12,
    InteriorBodyStageSaved = 13,
    ExteriorBodyStageSaved = 14,
    TireStageSaved = 15,
    AccessoryStageSaved = 16,
    MechanicalStageSaved = 17,
    RoadTestStageSaved = 18,
}
