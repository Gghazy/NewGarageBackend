using Garage.Domain.Common.Primitives;

namespace Garage.Domain.Employees.Entities;

public class Employee : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string NameAr { get; private set; }
    public string NameEn { get; private set; }

    private readonly List<EmployeeBranch> _branches = new();
    public IReadOnlyCollection<EmployeeBranch> Branches => _branches.AsReadOnly();

    private Employee() { }

    public Employee(Guid userId, string nameAr, string nameEn, List<Guid> branchIds)
    {
        UserId = userId;
        NameAr = nameAr;
        NameEn = nameEn;
        SetBranches(branchIds);
    }

    public void Update(string nameAr, string nameEn, List<Guid> branchIds)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        SetBranches(branchIds);
    }

    public void UpdateName(string nameAr, string nameEn)
    {
        NameAr = nameAr;
        NameEn = nameEn;
    }

    private void SetBranches(List<Guid> branchIds)
    {
        _branches.Clear();
        foreach (var branchId in branchIds.Distinct())
        {
            _branches.Add(new EmployeeBranch(Id, branchId));
        }
    }
}
