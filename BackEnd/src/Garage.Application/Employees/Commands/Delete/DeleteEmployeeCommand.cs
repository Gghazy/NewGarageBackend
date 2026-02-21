using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Employees.Commands.Delete;

public record DeleteEmployeeCommand(Guid Id) : IRequest<Result<bool>>;
