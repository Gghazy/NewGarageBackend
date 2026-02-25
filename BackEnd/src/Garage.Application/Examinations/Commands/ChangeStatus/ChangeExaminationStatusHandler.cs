using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.ChangeStatus;

public sealed class StartExaminationHandler : BaseCommandHandler<StartExaminationCommand, Guid>
{
    private readonly IRepository<Examination> _repo;
    private readonly IUnitOfWork              _unitOfWork;

    public StartExaminationHandler(IRepository<Examination> repo, IUnitOfWork unitOfWork)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(StartExaminationCommand command, CancellationToken ct)
    {
        var examination = await _repo.QueryTracking()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try
        {
            var invoiceNumber = await GenerateInvoiceNumber(ct);
            examination.SetInvoiceNumber(invoiceNumber);
            examination.Start();
        }
        catch (Exception ex) { return Fail(ex.Message); }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }

    private async Task<string> GenerateInvoiceNumber(CancellationToken ct)
    {
        var year   = DateTime.UtcNow.Year;
        var prefix = $"INV-{year}-";

        var lastNumber = await _repo.Query()
            .Where(e => e.InvoiceNumber != null && e.InvoiceNumber.StartsWith(prefix))
            .Select(e => e.InvoiceNumber!)
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

public sealed class CompleteExaminationHandler : BaseCommandHandler<CompleteExaminationCommand, Guid>
{
    private readonly IRepository<Examination> _repo;
    private readonly IUnitOfWork              _unitOfWork;

    public CompleteExaminationHandler(IRepository<Examination> repo, IUnitOfWork unitOfWork)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CompleteExaminationCommand command, CancellationToken ct)
    {
        var examination = await _repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Complete(); }
        catch (Exception ex) { return Fail(ex.Message); }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}

public sealed class DeliverExaminationHandler : BaseCommandHandler<DeliverExaminationCommand, Guid>
{
    private readonly IRepository<Examination> _repo;
    private readonly IUnitOfWork              _unitOfWork;

    public DeliverExaminationHandler(IRepository<Examination> repo, IUnitOfWork unitOfWork)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(DeliverExaminationCommand command, CancellationToken ct)
    {
        var examination = await _repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Deliver(); }
        catch (Exception ex) { return Fail(ex.Message); }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}

public sealed class CancelExaminationHandler : BaseCommandHandler<CancelExaminationCommand, Guid>
{
    private readonly IRepository<Examination> _repo;
    private readonly IUnitOfWork              _unitOfWork;

    public CancelExaminationHandler(IRepository<Examination> repo, IUnitOfWork unitOfWork)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CancelExaminationCommand command, CancellationToken ct)
    {
        var examination = await _repo.QueryTracking()
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null) return Fail("Examination not found.");

        try   { examination.Cancel(command.Reason); }
        catch (Exception ex) { return Fail(ex.Message); }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(examination.Id);
    }
}
