using Garage.Application.Abstractions;
using Garage.Contracts.Terms;
using Garage.Domain.Terms.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Terms.Queries.GetById;

public sealed class GetTermsByIdHandler(IReadRepository<Term> repo)
 : IRequestHandler<GetTermsByIdQuery, TermsDto>
{
    public async Task<TermsDto> Handle(GetTermsByIdQuery request, CancellationToken ct)
    {
        var term =
            await repo.Query()
            .Select(t => new TermsDto(t.Id, t.TermsAndConditionsAr, t.TermsAndConditionsEn, t.CancelWarrantyDocumentAr, t.CancelWarrantyDocumentEn))
                      .FirstOrDefaultAsync(ct)
                     ?? new TermsDto(null, "", "", "", "");

        return term;
    }
}

