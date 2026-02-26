using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Invoices.Commands.SetDiscount;

public sealed record SetInvoiceDiscountCommand(Guid InvoiceId, decimal Amount)
    : IRequest<Result<Guid>>;
