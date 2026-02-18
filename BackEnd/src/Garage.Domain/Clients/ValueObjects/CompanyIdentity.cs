using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Clients.ValueObjects;

public sealed class CompanyIdentity : ValueObject
{
    public string CommercialRegister { get; }
    public string TaxNumber { get; }

    private CompanyIdentity() { } 

    public CompanyIdentity(string commercialRegister, string taxNumber)
    {
        if (string.IsNullOrWhiteSpace(commercialRegister))
            throw new DomainException("Commercial Register is required");

        if (string.IsNullOrWhiteSpace(taxNumber))
            throw new DomainException("Tax Number is required");

        CommercialRegister = commercialRegister.Trim();
        TaxNumber = taxNumber.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CommercialRegister;
        yield return TaxNumber;
    }
}
