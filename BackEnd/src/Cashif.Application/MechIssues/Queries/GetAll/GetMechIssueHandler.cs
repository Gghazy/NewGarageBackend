using Cashif.Application.Abstractions;
using Cashif.Application.Branches.Queries.GetAll;
using Cashif.Application.Common.Extensions;
using Cashif.Contracts.Branches;
using Cashif.Contracts.Common;
using Cashif.Contracts.MechIssues;
using Cashif.Domain.Branches.Entities;
using Cashif.Domain.MechIssues.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.MechIssues.Queries.GetAll
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
