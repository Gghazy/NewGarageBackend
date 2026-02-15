using Garage.Domain.Clients.Enums;
using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Clients.Entities
{
    public class Client:AggregateRoot
    {
        public Guid UserId { get; protected set; }      // Identity UserId
        public ClientType Type { get; protected set; }

        public string NameAr { get; protected set; } = null!;
        public string NameEn { get; protected set; } = null!;
    }
}
