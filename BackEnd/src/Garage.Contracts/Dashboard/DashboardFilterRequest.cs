namespace Garage.Contracts.Dashboard;

public sealed record DashboardFilterRequest(
    DateTime? From,
    DateTime? To,
    Guid? BranchId
);
