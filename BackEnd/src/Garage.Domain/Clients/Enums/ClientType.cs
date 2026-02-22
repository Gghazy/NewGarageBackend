using Garage.Domain.Common.Primitives;


namespace Garage.Domain.Clients.Enums;


public sealed class ClientType : SmartEnum<ClientType>
{
    public string NameAr { get; }
    public int DisplayOrder { get; }

    private ClientType(int id, string nameEn, string nameAr, int displayOrder)
        : base(nameEn, id)
    {
        NameAr = nameAr;
        DisplayOrder = displayOrder;
    }

    public static readonly ClientType Individual = new(1, "Individual", "فرد", 1);
    public static readonly ClientType Company = new(2, "Company", "شركة", 2);
    public static readonly ClientType Government = new(3, "Government", "جهة حكومية", 3);
}

