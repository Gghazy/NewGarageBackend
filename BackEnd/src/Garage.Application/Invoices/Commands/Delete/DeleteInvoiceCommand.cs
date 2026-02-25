using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Invoices.Commands.Delete;

public record DeleteInvoiceCommand(Guid Id) : IRequest<Result<bool>>;
