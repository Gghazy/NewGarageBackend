using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Clients;
using Garage.Contracts.Common;

using Garage.Domain.Clients.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Clients.Queries.GetAllClientsBySearch;

public class GetAllClientsBySearchQueryHandler(IApplicationDbContext _dbContext) : IRequestHandler<GetAllClientsBySearchQuery, QueryResult<ClientDto>>
{
    public async Task<QueryResult<ClientDto>> Handle(GetAllClientsBySearchQuery command, CancellationToken ct)
    {
        var query =
            from c in _dbContext.Clients.AsNoTracking()
            join u in _dbContext.Users.AsNoTracking()
                on c.UserId equals u.Id
            join cr in _dbContext.ClientResources.AsNoTracking()
                on c.ResourceId equals cr.Id
            select new ClientDto(
                c.Type.NameAr,
                c.Type.Name,
                c.NameAr,
                c.NameEn,
                c.PhoneNumber,
                (c as CompanyClient) != null  ? ((CompanyClient)c).Identity.TaxNumber : null,
                (c as CompanyClient) != null ? ((CompanyClient)c).Identity.CommercialRegister : null,
                u.Email!,
                cr.NameEn,
                cr.NameAr
            );

        return await query.ToQueryResult(command.Search.CurrentPage, command.Search.ItemsPerPage, ct: ct);
    }

}


