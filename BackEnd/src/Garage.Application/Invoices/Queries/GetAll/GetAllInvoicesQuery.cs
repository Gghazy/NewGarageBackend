using Garage.Contracts.Common;
using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Queries.GetAll;

public sealed record GetAllInvoicesQuery(SearchCriteria Search) : IRequest<QueryResult<InvoiceDto>>;
