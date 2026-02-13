using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Domain.Terms.Entity;
using MediatR;
 


namespace Garage.Application.Terms.Commands.Create;

public sealed class CreateTermHandler(IRepository<Term> repo, IUnitOfWork uow) : IRequestHandler<CreateTermCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateTermCommand command, CancellationToken ct)
    {
        var entity = new Term(command.Request.TermsAndCondtionsAr, command.Request.TermsAndCondtionsEn, command.Request.CancelWarrantyDocumentAr, command.Request.CancelWarrantyDocumentEn);

        await repo.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return Result<Guid>.Ok(entity.Id);
    }
}

