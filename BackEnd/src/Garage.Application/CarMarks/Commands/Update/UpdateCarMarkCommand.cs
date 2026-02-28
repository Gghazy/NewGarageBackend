using Garage.Contracts.CarMarks;
using MediatR;

namespace Garage.Application.CarMarks.Commands.Update;

public sealed record UpdateCarMarkCommand(Guid Id, CarMarkRequest Request) : IRequest<bool>;
