namespace Garage.Contracts.Auth;
public record RegisterUserRequest(string Email, string Password, string? Phone);

