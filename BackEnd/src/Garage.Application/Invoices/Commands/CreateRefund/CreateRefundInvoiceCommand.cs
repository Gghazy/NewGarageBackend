using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Invoices.Commands.CreateRefund;

public sealed record CreateRefundInvoiceCommand(Guid OriginalInvoiceId) : IRequest<Result<Guid>>;
