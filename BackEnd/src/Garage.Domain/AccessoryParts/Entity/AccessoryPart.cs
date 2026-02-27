using Garage.Domain.Common.Primitives;

namespace Garage.Domain.AccessoryParts.Entity;

public class AccessoryPart : LookupBase
{
    private AccessoryPart() { }
    public AccessoryPart(string nameAr, string nameEn) : base(nameAr, nameEn) { }
}
