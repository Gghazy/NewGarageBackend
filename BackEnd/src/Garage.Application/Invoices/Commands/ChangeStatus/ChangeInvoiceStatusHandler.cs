using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.ChangeStatus;

public sealed class IssueInvoiceHandler : BaseCommandHandler<IssueInvoiceCommand, Guid>
{
    private readonly IRepository<Invoice> _repo;
    private readonly IUnitOfWork          _unitOfWork;

    public IssueInvoiceHandler(IRepository<Invoice> repo, IUnitOfWork unitOfWork)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(IssueInvoiceCommand command, CancellationToken ct)
    {
        var invoice = await _repo.QueryTracking()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == command.Id, ct);

        if (invoice is null) return Fail("Invoice not found.");

        try
        {
            var invoiceNumber = await GenerateInvoiceNumber(ct);
            invoice.SetInvoiceNumber(invoiceNumber);
            invoice.Issue();
        }
        catch (Exception ex) { return Fail(ex.Message); }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(invoice.Id);
    }

    private async Task<string> GenerateInvoiceNumber(CancellationToken ct)
    {
        var year   = DateTime.UtcNow.Year;
        var prefix = $"INV-{year}-";

        var lastNumber = await _repo.Query()
            .Where(i => i.InvoiceNumber != null && i.InvoiceNumber.StartsWith(prefix))
            .Select(i => i.InvoiceNumber!)
            .OrderByDescending(n => n)
            .FirstOrDefaultAsync(ct);

        var next = 1;
        if (lastNumber is not null)
        {
            var numPart = lastNumber.Substring(prefix.Length);
            if (int.TryParse(numPart, out var parsed))
                next = parsed + 1;
        }

        return $"{prefix}{next:D5}";
    }
}

public sealed class CancelInvoiceHandler : BaseCommandHandler<CancelInvoiceCommand, Guid>
{
    private readonly IRepository<Invoice> _repo;
    private readonly IUnitOfWork          _unitOfWork;

    public CancelInvoiceHandler(IRepository<Invoice> repo, IUnitOfWork unitOfWork)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CancelInvoiceCommand command, CancellationToken ct)
    {
        var invoice = await _repo.QueryTracking()
            .FirstOrDefaultAsync(i => i.Id == command.Id, ct);

        if (invoice is null) return Fail("Invoice not found.");

        try   { invoice.Cancel(command.Reason); }
        catch (Exception ex) { return Fail(ex.Message); }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(invoice.Id);
    }
}
