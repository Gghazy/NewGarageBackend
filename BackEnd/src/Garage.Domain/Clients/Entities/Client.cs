using Garage.Domain.Clients.Enums;
using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.Shared.ValueObjects;


namespace Garage.Domain.Clients.Entities;

public abstract class Client : AggregateRoot
{
    public Guid UserId { get; protected set; }
    public ClientType Type { get; protected set; }

    public string NameAr { get; protected set; } = null!;
    public string NameEn { get; protected set; } = null!;

    public Guid? ResourceId { get; protected set; }

    public string PhoneNumber { get; protected set; } = null!;

    public Address Address { get; private set; } = null!;


    protected Client() { }

    protected Client(Guid userId, ClientType type, string nameAr, string nameEn, string phoneNumber)
    {
        SetUser(userId);
        SetType(type);
        UpdateBaseData(nameAr, nameEn, phoneNumber);
    }

    protected void SetUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required");

        UserId = userId;
    }

    protected void SetType(ClientType type)
    {
        if (!Enum.IsDefined(typeof(ClientType), type))
            throw new DomainException("Invalid ClientType");

        Type = type;
    }

    public void UpdateBaseData(string nameAr, string nameEn, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("NameAr is required");

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("NameEn is required");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("PhoneNumber is required");

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        PhoneNumber = phoneNumber.Trim(); 
    }
}
