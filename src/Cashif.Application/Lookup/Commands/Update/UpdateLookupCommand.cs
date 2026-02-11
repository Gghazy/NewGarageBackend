using Cashif.Contracts.Lookup;
using Cashif.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.Lookup.Commands.Update
{
    public record UpdateLookupCommand<TEntity>(Guid Id, LookupRequest Req)
    : IRequest<bool>
    where TEntity : LookupBase;
}
