using Garage.Domain.Clients.Enums;
using Garage.Domain.Clients.ValueObjects;
using Garage.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Clients.Entities;

public sealed class CompanyClient : Client
{
    public CompanyIdentity Identity { get; private set; } = null!;
    public Address Address { get; private set; } = null!;

    private CompanyClient() { } // EF

    public CompanyClient(
        Guid userId,
        string nameAr,
        string nameEn,
        string phoneNumber,
        CompanyIdentity identity,
        Address address)
        : base(userId, ClientType.Company, nameAr, nameEn, phoneNumber)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void UpdateCompanyInfo(CompanyIdentity identity, Address address)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }
}

