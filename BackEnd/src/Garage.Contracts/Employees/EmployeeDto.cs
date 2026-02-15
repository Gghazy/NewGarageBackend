namespace Garage.Contracts.Employees;

public record EmployeeDto(
    Guid Id,
    Guid UserId,
    string NameAr,
    string NameEn,
    string RoleName,
    string PhoneNumber,
    string BranchNameEn,
    string BranchNameAr,
    bool IsActive,
    string Email
    );
