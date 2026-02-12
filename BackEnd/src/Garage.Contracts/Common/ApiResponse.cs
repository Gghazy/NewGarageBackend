namespace Garage.Contracts.Common;
public record ApiResponse<T>(T Data, string? Message = null);
public record ApiMessage(string Message);

