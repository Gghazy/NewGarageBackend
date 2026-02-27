using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Queries.GetByExamination;

public sealed record GetInvoicesByExaminationQuery(Guid ExaminationId) : IRequest<List<InvoiceDto>>;
