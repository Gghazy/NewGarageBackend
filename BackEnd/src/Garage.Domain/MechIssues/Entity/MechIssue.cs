using Garage.Domain.Common.Primitives;

namespace Garage.Domain.MechIssues.Entity;

public class MechIssue : LookupBase
{
    public MechIssue() { }
    public MechIssue(string nameAr, string nameEn) : base(nameAr, nameEn) { }
}
