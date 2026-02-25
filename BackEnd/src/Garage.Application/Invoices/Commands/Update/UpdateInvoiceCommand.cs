using Garage.Application.Common;
using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Commands.Update;

public sealed record UpdateInvoiceCommand(Guid Id, UpdateInvoiceRequest Request) : IRequest<Result<Guid>>;
