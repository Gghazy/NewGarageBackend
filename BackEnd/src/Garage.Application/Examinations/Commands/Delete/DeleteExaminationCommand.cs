using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Examinations.Commands.Delete;

public record DeleteExaminationCommand(Guid Id) : IRequest<Result<bool>>;
