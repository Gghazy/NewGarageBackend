
using Garage.Application.Abstractions;
using Garage.Contracts.Lookup;
using Garage.Domain.Clients.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Clients.Queries.GetAllClients;


public class GetAllClientsQueryHandler(IReadRepository<Client> repo) : IRequestHandler<GetAllClientsQuery, List<LookupDto>>
{
    public async Task<List<LookupDto>> Handle(GetAllClientsQuery request, CancellationToken ct)
    {
        var list = await repo.Query().Select(b => new LookupDto(b.Id, b.NameAr, b.NameEn)).ToListAsync();
        return list;
    }
}

