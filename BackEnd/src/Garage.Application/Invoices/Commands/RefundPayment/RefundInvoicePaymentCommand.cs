using Garage.Application.Common;
using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Commands.RefundPayment;

public sealed record RefundInvoicePaymentCommand(Guid InvoiceId, AddInvoicePaymentRequest Request)
    : IRequest<Result<Guid>>;
