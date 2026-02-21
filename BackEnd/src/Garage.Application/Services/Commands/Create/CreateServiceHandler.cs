using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Services.Entities;

namespace Garage.Application.Services.Commands.Create;

public sealed class CreateServiceHandler : BaseCommandHandler<CreateServiceCommand, Guid>
{
    private readonly IRepository<Service> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceHandler(IRepository<Service> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CreateServiceCommand request, CancellationToken ct)
    {
        var service = new Service(request.Request.NameAr, request.Request.NameEn);
        service.SetStages(request.Request.Stages);

        await _repository.AddAsync(service, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Ok(service.Id);
    }
}