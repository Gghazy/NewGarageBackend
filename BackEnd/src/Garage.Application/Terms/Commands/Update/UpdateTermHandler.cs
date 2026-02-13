using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Domain.Terms.Entity;
using MediatR;

namespace Garage.Application.Terms.Commands.Update;

public sealed class UpdateTermHandler(IRepository<Term> repo, IUnitOfWork uow) : IRequestHandler<UpdateTermCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateTermCommand command, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(command.Id, ct);
        if (entity is null)
            throw new KeyNotFoundException("Term not found");
        entity.Update(command.Request.TermsAndCondtionsAr, command.Request.TermsAndCondtionsEn, command.Request.CancelWarrantyDocumentAr, command.Request.CancelWarrantyDocumentEn);

        await uow.SaveChangesAsync(ct);
           return Result<Guid>.Ok(entity.Id); ;
    }
}

