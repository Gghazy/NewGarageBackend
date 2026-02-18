namespace Garage.Contracts.Employees;

public record EmployeeDto(
    Guid Id,
    Guid UserId,
    string NameAr,
    string NameEn,
    Guid RoleId,
    string RoleName,
    string PhoneNumber,
    Guid BranchId,
    string BranchNameEn,
    string BranchNameAr,
    bool IsActive,
    string Email
    );
