using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.SaveDashboardIndicatorsStage;

public sealed class SaveDashboardIndicatorsStageHandler(
    IRepository<Examination> examRepo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<SaveDashboardIndicatorsStageCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(SaveDashboardIndicatorsStageCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var exam = await examRepo.QueryTracking()
            .Include(x => x.DashboardIndicatorsStageResult)
                .ThenInclude(s => s!.Items)
            .FirstOrDefaultAsync(x => x.Id == command.ExaminationId, ct);

        if (exam is null)
            return Fail("Examination not found.");

        var indicators = req.Indicators
            .Select(i => (i.Key, i.Value, i.NotApplicable));

        exam.SaveDashboardIndicatorsStage(req.Comments, indicators);

        await examRepo.UpdateAsync(exam, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(exam.Id);
    }
}
