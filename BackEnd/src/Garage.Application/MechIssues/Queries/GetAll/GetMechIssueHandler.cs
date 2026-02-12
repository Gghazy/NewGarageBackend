using Garage.Application.Abstractions;
using Garage.Application.Branches.Queries.GetAll;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Branches;
using Garage.Contracts.Common;
using Garage.Contracts.MechIssues;
using Garage.Domain.Branches.Entities;
using Garage.Domain.MechIssues.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.MechIssues.Queries.GetAll
{

    public class GetMechIssueHandler(IReadRepository<MechIssue> repo) : IRequestHandler<GetMechIssueQuery, QueryResult<MechIssueResponse>>
    {
        public async Task<QueryResult<MechIssueResponse>> Handle(GetMechIssueQuery request, CancellationToken ct)
        {
            var list = await repo.Query()
                .Include(x => x.MechIssueType)
                .Select(b => new MechIssueResponse(
                    b.Id,
                    b.NameAr,
                    b.NameEn,
                    b.MechIssueType.NameEn,
                    b.MechIssueType.NameAr,
                    b.MechIssueTypeId
                    )).ToQueryResult(request.Search.CurrentPage, request.Search.ItemsPerPage);
            return list;
        }
    }
}

