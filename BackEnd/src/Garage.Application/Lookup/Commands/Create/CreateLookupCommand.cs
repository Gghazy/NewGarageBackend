using Garage.Contracts.Lookup;
using Garage.Domain.Common.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Lookup.Commands.Create
{
    public record CreateLookupCommand<TEntity>(LookupRequest Req)
     : IRequest<Guid>
     where TEntity : LookupBase;

}

