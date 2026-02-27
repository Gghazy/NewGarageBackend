using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.SaveInteriorBodyStage;

public sealed class SaveInteriorBodyStageHandler(
    IRepository<Examination> examRepo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<SaveInteriorBodyStageCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(SaveInteriorBodyStageCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var exam = await examRepo.QueryTracking()
            .Include(x => x.InteriorBodyStageResult)
                .ThenInclude(s => s!.Items)
            .FirstOrDefaultAsync(x => x.Id == command.ExaminationId, ct);

        if (exam is null)
            return Fail("Examination not found.");

        var items = req.Items
            .Select(i => (i.PartId, i.IssueId));

        exam.SaveInteriorBodyStage(req.NoIssuesFound, req.Comments, items);

        await examRepo.UpdateAsync(exam, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(exam.Id);
    }
}
