using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Examinations.Commands.ChangeStatus;

public sealed record StartExaminationCommand(Guid Id)    : IRequest<Result<Guid>>;
public sealed record CompleteExaminationCommand(Guid Id) : IRequest<Result<Guid>>;
public sealed record DeliverExaminationCommand(Guid Id)  : IRequest<Result<Guid>>;
public sealed record CancelExaminationCommand(Guid Id, string? Reason) : IRequest<Result<Guid>>;
