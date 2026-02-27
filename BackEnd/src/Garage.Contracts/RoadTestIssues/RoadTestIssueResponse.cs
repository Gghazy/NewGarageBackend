namespace Garage.Contracts.RoadTestIssues;

public record RoadTestIssueResponse(
    Guid Id,
    string NameAr,
    string NameEn,
    string RoadTestIssueTypeNameAr,
    string RoadTestIssueTypeNameEn,
    Guid RoadTestIssueTypeId);
