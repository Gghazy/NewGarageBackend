using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.MechParts;
using Garage.Domain.MechParts.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.MechParts.Queries.GetAll
{

    public class GetMechPartHandler(IReadRepository<MechPart> repo) : IRequestHandler<GetMechPartQuery, QueryResult<MechPartResponse>>
    {
        public async Task<QueryResult<MechPartResponse>> Handle(GetMechPartQuery request, CancellationToken ct)
        {
            var list = await repo.Query()
                .Include(x => x.MechPartType)
                .Select(b => new MechPartResponse(
                    b.Id,
                    b.NameAr,
                    b.NameEn,
                    b.MechPartType.NameEn,
                    b.MechPartType.NameAr,
                    b.MechPartTypeId
                    )).ToQueryResult(request.Search.CurrentPage, request.Search.ItemsPerPage);
            return list;
        }
    }
}
