using Garage.Contracts.Examinations;

namespace Garage.Application.Abstractions;

public interface IExaminationReportPdfService
{
    byte[] Generate(ExaminationReportDto report);
}
