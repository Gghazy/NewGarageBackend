using Garage.Contracts.Employees;
using MediatR;


namespace Garage.Application.Employees.Commands.Create;

public sealed record CreateEmployeeCommand(EmployeeRequest Request) : IRequest<Guid>;
