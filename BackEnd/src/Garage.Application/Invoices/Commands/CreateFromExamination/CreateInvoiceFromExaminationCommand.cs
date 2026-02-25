using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Invoices.Commands.CreateFromExamination;

public sealed record CreateInvoiceFromExaminationCommand(Guid ExaminationId)
    : IRequest<Result<Guid>>;
