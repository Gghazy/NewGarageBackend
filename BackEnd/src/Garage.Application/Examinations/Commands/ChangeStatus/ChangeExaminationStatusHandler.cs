using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.ChangeStatus;

public sealed class StartExaminationHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<StartExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(StartExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Start(); }
        catch (Exception ex) { return Fail(ex.Message); }

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}

public sealed class CompleteExaminationHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<CompleteExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CompleteExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Complete(); }
        catch (Exception ex) { return Fail(ex.Message); }

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}

public sealed class DeliverExaminationHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<DeliverExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(DeliverExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Deliver(); }
        catch (Exception ex) { return Fail(ex.Message); }

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}

public sealed class CancelExaminationHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<CancelExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CancelExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Cancel(command.Reason); }
        catch (Exception ex) { return Fail(ex.Message); }

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}
