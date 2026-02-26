using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Invoices.Commands.ChangeStatus;

public sealed record CancelInvoiceCommand(Guid Id, string? Reason) : IRequest<Result<Guid>>;
