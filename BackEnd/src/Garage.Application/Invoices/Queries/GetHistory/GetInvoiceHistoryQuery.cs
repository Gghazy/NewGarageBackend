using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Queries.GetHistory;

public sealed record GetInvoiceHistoryQuery(Guid InvoiceId)
    : IRequest<List<InvoiceHistoryDto>>;
