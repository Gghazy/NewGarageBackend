using Garage.Contracts.Employees;
using MediatR;


namespace Garage.Application.Employees.Commands.Update
{
    public sealed record UpdateEmployeeCommand(Guid Id, EmployeeRequest Request) : IRequest<Guid>;
}
