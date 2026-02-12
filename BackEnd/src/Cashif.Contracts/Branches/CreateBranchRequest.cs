namespace Cashif.Contracts.Branches;
public record CreateBranchRequest(string NameAr, string NameEn, bool IsActive = true);
