using Garage.Domain.Common.Primitives;

namespace Garage.Domain.Employees.Entities;

public sealed class EmployeeBranch : Entity
{
    public Guid EmployeeId { get; private set; }
    public Guid BranchId { get; private set; }

    private EmployeeBranch() { }

    internal EmployeeBranch(Guid employeeId, Guid branchId)
    {
        EmployeeId = employeeId;
        BranchId = branchId;
    }
}
