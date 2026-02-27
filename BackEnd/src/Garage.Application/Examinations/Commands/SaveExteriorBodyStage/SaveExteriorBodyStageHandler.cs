using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.SaveExteriorBodyStage;

public sealed class SaveExteriorBodyStageHandler(
    IRepository<Examination> examRepo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<SaveExteriorBodyStageCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(SaveExteriorBodyStageCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var exam = await examRepo.QueryTracking()
            .Include(x => x.ExteriorBodyStageResult)
                .ThenInclude(s => s!.Items)
            .FirstOrDefaultAsync(x => x.Id == command.ExaminationId, ct);

        if (exam is null)
            return Fail("Examination not found.");

        var items = req.Items
            .Select(i => (i.PartId, i.IssueId));

        exam.SaveExteriorBodyStage(req.NoIssuesFound, req.Comments, items);

        await examRepo.UpdateAsync(exam, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(exam.Id);
    }
}
