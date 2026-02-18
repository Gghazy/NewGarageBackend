using Garage.Domain.Clients.Enums;
using Garage.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Clients.Entities
{
    public sealed class IndividualClient : Client
    {
        public Address Address { get; private set; } = null!;

        private IndividualClient() { }

        public IndividualClient(
            Guid userId,
            string nameAr,
            string nameEn,
            string phoneNumber,
            Address address)
            : base(userId, ClientType.Individual, nameAr, nameEn, phoneNumber)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public void UpdatePersonalInfo( Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }

}
