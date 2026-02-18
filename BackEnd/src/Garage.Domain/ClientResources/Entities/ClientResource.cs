using Garage.Domain.Common.Primitives;


namespace Garage.Domain.ClientResources.Entities;

public class ClientResource : LookupBase
{
    public ClientResource()
    {

    }
    public ClientResource(string NameAr, string NameEn) : base(NameAr, NameEn) { }
}