using Garage.Domain.Clients.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Clients.Entities
{
    public class CompanyClient:Client
    {
        public string CommercialRegister { get; private set; } = null!;
        public string TaxNumber { get; private set; } = null!;
        public string? ContactPerson { get; private set; }

        private CompanyClient() { }

        public CompanyClient(Guid id, Guid userId, string nameAr, string nameEn, string cr, string taxNumber)
        {
            Id = id;
            UserId = userId;
            Type = ClientType.Company;
            NameAr = nameAr;
            NameEn = nameEn;
            CommercialRegister = cr;
            TaxNumber = taxNumber;
        }
    }
}
