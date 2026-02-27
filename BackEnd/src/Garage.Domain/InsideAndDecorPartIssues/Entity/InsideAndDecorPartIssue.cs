using Garage.Domain.Common.Primitives;

namespace Garage.Domain.InsideAndDecorPartIssues.Entity;

public class InsideAndDecorPartIssue : LookupBase
{
    private InsideAndDecorPartIssue() { }
    public InsideAndDecorPartIssue(string nameAr, string nameEn) : base(nameAr, nameEn) { }
}
