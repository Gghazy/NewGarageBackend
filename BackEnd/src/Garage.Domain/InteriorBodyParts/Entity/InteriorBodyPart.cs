using Garage.Domain.Common.Primitives;

namespace Garage.Domain.InteriorBodyParts.Entity;

public class InteriorBodyPart : LookupBase
{
    private InteriorBodyPart() { }
    public InteriorBodyPart(string nameAr, string nameEn) : base(nameAr, nameEn) { }
}
