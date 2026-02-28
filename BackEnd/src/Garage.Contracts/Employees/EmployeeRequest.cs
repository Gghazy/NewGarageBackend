namespace Garage.Contracts.Employees;

public record EmployeeRequest(
    Guid UserId,
    string NameAr,
    string NameEn,
    string PhoneNumber,
    string Email,
    List<Guid> BranchIds,
    Guid RoleId);
