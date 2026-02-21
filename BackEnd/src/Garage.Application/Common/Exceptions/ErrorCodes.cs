namespace Garage.Application.Common.Exceptions;

/// <summary>
/// Constants for error codes used throughout the application
/// </summary>
public static class ErrorCodes
{
    // Authentication & Authorization
    public const string AuthUnauthorized = "Auth.Unauthorized";
    public const string AuthInvalidCredentials = "Auth.InvalidCredentials";
    public const string AuthTokenExpired = "Auth.TokenExpired";
    public const string AuthInvalidToken = "Auth.InvalidToken";
    public const string AuthAccessDenied = "Auth.AccessDenied";
    public const string AuthForbidden = "Auth.Forbidden";

    // Common
    public const string NotFound = "NotFound";
    public const string Success = "Common.Success";
    public const string Deleted = "Common.Deleted";

    // Validation
    public const string ValidationError = "Validation.Error";
    public const string ValidationRequired = "Validation.Required";
    public const string ValidationInvalidFormat = "Validation.InvalidFormat";
    public const string ValidationInvalidEmail = "Validation.InvalidEmail";
    public const string ValidationBadArgument = "Validation.BadArgument";

    // Operations
    public const string OperationInvalid = "Operation.InvalidOperation";
    public const string OperationFailed = "Operation.OperationFailed";

    // Server
    public const string ServerError = "Server.Error";
    public const string ServerInternalError = "Server.InternalError";

    // Branch
    public const string BranchNotFound = "Branch.NotFound";
    public const string BranchExists = "Branch.Exists";
    public const string BranchCreated = "Branch.Created";
    public const string BranchUpdated = "Branch.Updated";
    public const string BranchDeleted = "Branch.Deleted";

    // Client
    public const string ClientNotFound = "Client.NotFound";
    public const string ClientExists = "Client.Exists";
    public const string ClientCreated = "Client.Created";
    public const string ClientUpdated = "Client.Updated";
    public const string ClientDeleted = "Client.Deleted";

    // User
    public const string UserNotFound = "User.NotFound";
    public const string UserExists = "User.Exists";
    public const string UserCreated = "User.Created";
    public const string UserUpdated = "User.Updated";
    public const string UserDeleted = "User.Deleted";
    public const string UserPasswordChanged = "User.PasswordChanged";

    // Employee
    public const string EmployeeNotFound = "Employee.NotFound";
    public const string EmployeeExists = "Employee.Exists";
    public const string EmployeeCreated = "Employee.Created";
    public const string EmployeeUpdated = "Employee.Updated";
    public const string EmployeeDeleted = "Employee.Deleted";

    // Service
    public const string ServiceNotFound = "Service.NotFound";
    public const string ServiceCreated = "Service.Created";
    public const string ServiceUpdated = "Service.Updated";
    public const string ServiceDeleted = "Service.Deleted";

    // Mechanical Issue
    public const string MechIssueNotFound = "MechIssue.NotFound";
    public const string MechIssueCreated = "MechIssue.Created";
    public const string MechIssueUpdated = "MechIssue.Updated";
    public const string MechIssueDeleted = "MechIssue.Deleted";
}
