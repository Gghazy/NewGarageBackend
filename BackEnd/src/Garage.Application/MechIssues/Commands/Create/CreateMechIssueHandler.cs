using Garage.Application.Abstractions;
using Garage.Application.Branches.Commands.Create;
using Garage.Application.Common;
using Garage.Domain.Branches.Entities;
using Garage.Domain.MechIssues.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.MechIssues.Commands.Create
{

    public class CreateMechIssueHandler(IRepository<MechIssue> repo, IUnitOfWork uow) : IRequestHandler<CreateMechIssueCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateMechIssueCommand request, CancellationToken ct)
        {
            var req = request.Request;
            var entity = new MechIssue(req.NameAr, req.NameEn,req.MechIssueTypeId);
            await repo.AddAsync(entity, ct);
            await uow.SaveChangesAsync(ct);
            return Result<Guid>.Ok(entity.Id);
        }
    }
}

