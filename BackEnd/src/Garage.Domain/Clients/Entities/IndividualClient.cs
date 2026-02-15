using Garage.Domain.Clients.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Clients.Entities
{
    public class IndividualClient:Client
    {
        public string NationalId { get; private set; } = null!;
        public DateOnly? BirthDate { get; private set; }

        private IndividualClient() { }

        public IndividualClient(Guid id, Guid userId, string nameAr, string nameEn, string nationalId)
        {
            Id = id;
            UserId = userId;
            Type = ClientType.Individual;
            NameAr = nameAr;
            NameEn = nameEn;
            NationalId = nationalId;
        }
    }
}
