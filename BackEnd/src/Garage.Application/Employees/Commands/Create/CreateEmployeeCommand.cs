using Garage.Contracts.Employees;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Employees.Commands.Create
{
   public sealed record CreateEmployeeCommand(EmployeeRequest Request) : IRequest<Guid>;
}
