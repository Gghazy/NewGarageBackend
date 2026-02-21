using Garage.Application.Common;
using Garage.Contracts.Services;
using MediatR;

namespace Garage.Application.Services.Commands.Create;

public sealed record CreateServiceCommand(CreateServiceRequest Request) : IRequest<Result<Guid>>;
