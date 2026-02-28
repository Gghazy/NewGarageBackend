using Garage.Application.Common;
using Garage.Contracts.CarMarks;
using MediatR;

namespace Garage.Application.CarMarks.Commands.Create;

public sealed record CreateCarMarkCommand(CarMarkRequest Request) : IRequest<Result<Guid>>;
