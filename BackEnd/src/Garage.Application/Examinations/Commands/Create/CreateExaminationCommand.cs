using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.Create;

public sealed record CreateExaminationCommand(CreateExaminationRequest Request) : IRequest<Result<Guid>>;
