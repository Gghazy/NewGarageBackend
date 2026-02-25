using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Queries.GetById;

public sealed record GetInvoiceByIdQuery(Guid Id) : IRequest<InvoiceDto?>;
