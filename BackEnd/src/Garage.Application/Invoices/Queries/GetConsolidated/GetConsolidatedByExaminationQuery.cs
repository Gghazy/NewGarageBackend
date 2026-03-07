using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Queries.GetConsolidated;

public sealed record GetConsolidatedByExaminationQuery(Guid ExaminationId)
    : IRequest<ConsolidatedInvoiceResponse?>;
