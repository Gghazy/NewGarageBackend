using Cashif.Application.Abstractions;
using Cashif.Application.Branches.Commands.Update;
using Cashif.Application.Common;
using Cashif.Domain.Branches.Entities;
using Cashif.Domain.MechIssues.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.MechIssues.Commands.Update
{

    public class UpdateMechIssueHandler(IRepository<MechIssue> repo, IUnitOfWork uow) : IRequestHandler<UpdateMechIssueCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateMechIssueCommand request, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(request.Id, ct);
            if (entity is null) return Result<bool>.Fail("Not found");

            entity.Update(request.Request.NameAr, request.Request.NameEn,request.Request.MechIssueTypeId);


            await repo.UpdateAsync(entity, ct);
            await uow.SaveChangesAsync(ct);
            return Result<bool>.Ok(true);
        }
    }
}
