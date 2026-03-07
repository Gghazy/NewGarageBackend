using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Application.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;
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

public sealed class BeginWorkExaminationHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<BeginWorkExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(BeginWorkExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.BeginWork(); }
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

public sealed class ReopenExaminationHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<ReopenExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(ReopenExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Reopen(); }
        catch (Exception ex) { return Fail(ex.Message); }

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}

public sealed class CancelExaminationHandler(
    IRepository<Examination> repo,
    IRepository<Invoice> invoiceRepo,
    IUnitOfWork unitOfWork,
    InvoiceNumberGenerator invoiceNumberGenerator)
    : BaseCommandHandler<CancelExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CancelExaminationCommand command, CancellationToken ct)
    {
        var examination = await repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Cancel(command.Reason); }
        catch (Exception ex) { return Fail(ex.Message); }

        var invoices = await invoiceRepo.QueryTracking()
            .Include(i => i.Items)
            .Where(i => i.ExaminationId == examination.Id
                     && i.Status != InvoiceStatus.Cancelled
                     && i.Type == InvoiceType.Invoice)
            .ToListAsync(ct);

        foreach (var invoice in invoices)
        {
            if (invoice.Status == InvoiceStatus.Paid)
            {
                // Create refund invoice for paid invoices, keep original status as-is
                var refund = Invoice.CreateEmptyRefundInvoice(invoice);
                foreach (var item in invoice.Items)
                    refund.AddItem(item.Description, item.TotalPrice, item.ServiceId, item.ServiceNameAr, item.ServiceNameEn, item.TotalPrice.Amount);

                refund.MarkAsRefunded();

                var refNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Refund, ct);
                refund.SetInvoiceNumber(refNumber);
                await invoiceRepo.AddAsync(refund, ct);
            }
            else
            {
                invoice.ForceCancel(command.Reason);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}
