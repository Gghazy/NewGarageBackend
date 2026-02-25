using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Shared;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.RefundPayment;

public sealed class RefundPaymentHandler(
    IRepository<Examination> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<RefundPaymentCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(RefundPaymentCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var examination = await repo.QueryTracking()
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.Id == command.ExaminationId, ct);

        if (examination is null)
            return Fail("Examination not found.");

        if (!Enum.TryParse<PaymentMethod>(req.Method, ignoreCase: true, out var method))
            return Fail($"Invalid payment method '{req.Method}'. Use Cash, Card, BankTransfer or Cheque.");

        var currency = string.IsNullOrWhiteSpace(req.Currency) ? "EGP" : req.Currency;
        var amount = Money.Create(req.Amount, currency);

        try
        {
            examination.AddRefund(amount, method, req.Notes);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var payment = examination.Payments.Last();
        return Ok(payment.Id);
    }
}
