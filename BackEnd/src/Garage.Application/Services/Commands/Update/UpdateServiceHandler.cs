using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Services.Entities;
using Garage.Domain.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Services.Commands.Update;

public sealed class UpdateServiceHandler : BaseCommandHandler<UpdateServiceCommand, bool>
{
    private readonly IRepository<Service> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceHandler(IRepository<Service> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<bool>> Handle(UpdateServiceCommand request, CancellationToken ct)
    {
        var service = await _repository.QueryTracking()
            .Include(x => x.Stages)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        if (service is null)
            return Fail(NotFoundError);

        service.SetNames(request.Request.NameAr, request.Request.NameEn);

        if (request.Request.Stages is null || request.Request.Stages.Count == 0)
            return Fail("Stages are required");

        foreach (var s in request.Request.Stages)
            _ = Stage.FromValue(s);

        if (request.Request.Stages.GroupBy(x => x).Any(g => g.Count() > 1))
            return Fail("Duplicate stages are not allowed");

        service.SetStages(request.Request.Stages);
        await _unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
