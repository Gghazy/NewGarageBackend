namespace Garage.Contracts.Examinations;

public sealed record DashboardIndicatorsStageResultDto(
    string? Comments,
    List<DashboardIndicatorDto> Indicators
);

public sealed record DashboardIndicatorDto(string Key, decimal? Value, bool NotApplicable);
