using Cashif.Contracts.Lookup;
using Cashif.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.Lookup.Commands.Create
{
    public record CreateLookupCommand<TEntity>(LookupRequest Req)
     : IRequest<Guid>
     where TEntity : LookupBase;

}
