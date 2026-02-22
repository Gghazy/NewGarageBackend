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
    join u in _dbContext.Users.AsNoTracking() on c.UserId equals u.Id
    join cr0 in _dbContext.ClientResources.AsNoTracking() on c.ResourceId equals cr0.Id into crJoin
    from cr in crJoin.DefaultIfEmpty()
    let company = c as CompanyClient
    let individual = c as IndividualClient
    select new ClientDto(
        c.Id,
        c.Type.NameAr,
        c.Type.Name,
        c.NameAr,
        c.NameEn,
        c.PhoneNumber,
        company != null ? company.Identity.TaxNumber : null,
        company != null ? company.Identity.CommercialRegister : null,
        u.Email!,
        cr != null ? cr.NameEn : null,
        cr != null ? cr.NameAr : null,
        cr != null ? cr.Id : null,
        individual != null ? individual.Address : null,
        company != null ? company.Address.StreetName : null,
        company != null ? company.Address.AdditionalStreetName : null,
        company != null ? company.Address.CityName : null,
        company != null ? company.Address.PostalZone : null,
        company != null ? company.Address.CountrySubentity : null,
        company != null ? company.Address.CountryCode : null,
        company != null ? company.Address.BuildingNumber : null,
        company != null ? company.Address.CitySubdivisionName : null
    );

        return await query.ToQueryResult(command.Search.CurrentPage, command.Search.ItemsPerPage, ct: ct);
    }

}


