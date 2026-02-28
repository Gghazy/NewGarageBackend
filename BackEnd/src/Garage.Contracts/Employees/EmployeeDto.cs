namespace Garage.Contracts.Employees;

public record EmployeeDto(
    Guid Id,
    Guid UserId,
    string NameAr,
    string NameEn,
    Guid RoleId,
    string RoleName,
    string PhoneNumber,
    List<EmployeeBranchDto> Branches,
    bool IsActive,
    string Email);
