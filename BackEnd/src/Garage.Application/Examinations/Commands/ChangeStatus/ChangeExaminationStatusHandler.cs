using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Application.Invoices;
using Garage.Domain.ExaminationManagement.Shared;
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
    InvoiceNumberGenerator invoiceNumberGenerator,
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

        await CreateRefundIfPaid(examination.Id, ct);

        await unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }

    private async Task CreateRefundIfPaid(Guid examinationId, CancellationToken ct)
    {
        // Find the original invoice for this examination
        var originalInvoice = await invoiceRepo.QueryTracking()
            .FirstOrDefaultAsync(i => i.ExaminationId == examinationId
                                   && i.Type == InvoiceType.Invoice
                                   && i.Status != InvoiceStatus.Cancelled, ct);

        if (originalInvoice is null || originalInvoice.Status != InvoiceStatus.Paid)
            return;

        // Find related adjustment and refund invoices
        var relatedInvoices = await invoiceRepo.QueryTracking()
            .Where(i => i.OriginalInvoiceId == originalInvoice.Id
                     && i.Status != InvoiceStatus.Cancelled)
            .ToListAsync(ct);

        // Calculate net amount to refund
        var netRefund = originalInvoice.TotalWithTax.Amount;

        // Add paid adjustment invoices (additional charges the customer paid)
        netRefund += relatedInvoices
            .Where(i => i.Type == InvoiceType.Adjustment && i.Status == InvoiceStatus.Paid)
            .Sum(i => i.TotalWithTax.Amount);

        // Subtract existing refund invoices (already refunded amounts)
        netRefund -= relatedInvoices
            .Where(i => i.Type == InvoiceType.Refund)
            .Sum(i => Math.Abs(i.TotalWithTax.Amount));

        if (netRefund <= 0) return;

        // Create refund invoice
        var refundInvoice = Invoice.CreateRefundInvoice(
            originalInvoice,
            Money.Create(netRefund, originalInvoice.TotalWithTax.Currency));

        var refNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Refund, ct);
        refundInvoice.SetInvoiceNumber(refNumber);

        await invoiceRepo.AddAsync(refundInvoice, ct);
    }
}
