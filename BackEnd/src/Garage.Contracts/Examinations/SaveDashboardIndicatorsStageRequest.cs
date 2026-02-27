namespace Garage.Contracts.Examinations;

public sealed record SaveDashboardIndicatorsStageRequest(
    string? Comments,
    List<DashboardIndicatorRequest> Indicators
);

public sealed record DashboardIndicatorRequest(string Key, decimal? Value, bool NotApplicable);
