using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.Update;

public sealed record UpdateExaminationCommand(Guid Id, UpdateExaminationRequest Request)
    : IRequest<Result<Guid>>;
