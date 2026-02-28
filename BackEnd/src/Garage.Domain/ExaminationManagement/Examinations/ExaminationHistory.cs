using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed class ExaminationHistory : Entity
{
    public Guid ExaminationId { get; private set; }
    public ExaminationHistoryAction Action { get; private set; }
    public string? Details { get; private set; }

    private ExaminationHistory() { }

    internal ExaminationHistory(
        Guid examinationId,
        ExaminationHistoryAction action,
        string? details = null)
    {
        ExaminationId = examinationId;
        Action = action;
        Details = details;
    }
}
