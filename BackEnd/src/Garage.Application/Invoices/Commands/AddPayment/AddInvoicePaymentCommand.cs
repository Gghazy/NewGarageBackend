using Garage.Application.Common;
using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Commands.AddPayment;

public sealed record AddInvoicePaymentCommand(Guid InvoiceId, AddInvoicePaymentRequest Request)
    : IRequest<Result<Guid>>;
