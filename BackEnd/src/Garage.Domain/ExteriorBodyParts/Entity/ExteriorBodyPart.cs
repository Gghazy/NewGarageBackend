using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ExteriorBodyParts.Entity;

public class ExteriorBodyPart : LookupBase
{
    private ExteriorBodyPart() { }
    public ExteriorBodyPart(string nameAr, string nameEn) : base(nameAr, nameEn) { }
}
