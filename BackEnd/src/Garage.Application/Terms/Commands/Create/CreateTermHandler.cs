using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Terms.Entity;

namespace Garage.Application.Terms.Commands.Create;

public sealed class CreateTermHandler : BaseCommandHandler<CreateTermCommand, Guid>
{
    private readonly IRepository<Term> _repo;
    private readonly IUnitOfWork _uow;

    public CreateTermHandler(IRepository<Term> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<Guid>> Handle(CreateTermCommand command, CancellationToken ct)
    {
        var entity = new Term(command.Request.TermsAndCondtionsAr, command.Request.TermsAndCondtionsEn, command.Request.CancelWarrantyDocumentAr, command.Request.CancelWarrantyDocumentEn);

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(entity.Id);
    }
}

