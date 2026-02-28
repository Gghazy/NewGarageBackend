using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Common;
using Garage.Contracts.Examinations;

namespace Garage.Application.Examinations.Queries.GetAll;

public sealed class GetAllExaminationsQueryHandler(
    IReadRepository<Examination> repo,
    IBranchAccessService branchAccess)
    : BaseQueryHandler<GetAllExaminationsQuery, QueryResult<ExaminationDto>>
{
    public override async Task<QueryResult<ExaminationDto>> Handle(GetAllExaminationsQuery request, CancellationToken ct)
    {
        var search = request.Search;
        var branchIds = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var query = repo.Query()
            .WhereBranchAccessible(branchIds)
            .WhereIf(
                !string.IsNullOrWhiteSpace(search.TextSearch),
                e => e.Client.NameAr.Contains(search.TextSearch!)   ||
                     e.Client.NameEn.Contains(search.TextSearch!)   ||
                     e.Client.PhoneNumber.Contains(search.TextSearch!))
            .Select(ExaminationProjection.ToDto);

        return await query.ToQueryResult(
            search.CurrentPage,
            search.ItemsPerPage,
            ct: ct);
    }
}
