using Garage.Application.Common;
using Garage.Contracts.Invoices;
using MediatR;

namespace Garage.Application.Invoices.Commands.Create;

public sealed record CreateInvoiceCommand(CreateInvoiceRequest Request) : IRequest<Result<Guid>>;
