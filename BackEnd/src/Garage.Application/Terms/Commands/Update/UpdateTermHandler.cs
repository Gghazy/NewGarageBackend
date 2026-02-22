using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Terms.Entity;

namespace Garage.Application.Terms.Commands.Update;

public sealed class UpdateTermHandler : BaseCommandHandler<UpdateTermCommand, Guid>
{
    private readonly IRepository<Term> _repo;
    private readonly IUnitOfWork _uow;

    public UpdateTermHandler(IRepository<Term> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<Guid>> Handle(UpdateTermCommand command, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(command.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        entity.Update(command.Request.TermsAndCondtionsAr, command.Request.TermsAndCondtionsEn, command.Request.CancelWarrantyDocumentAr, command.Request.CancelWarrantyDocumentEn);

        await _uow.SaveChangesAsync(ct);
        return Ok(entity.Id);
    }
}

