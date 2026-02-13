using Garage.Application.Abstractions;
using Garage.Domain.Common.Exceptions;
using Garage.Domain.Services.Entities;
using Garage.Domain.Services.Enums;
using MediatR;


namespace Garage.Application.Services.Commands.Update
{
    public sealed class UpdateServiceHandler( IRepository<Service> repo, IUnitOfWork uow) : IRequestHandler<UpdateServiceCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateServiceCommand request, CancellationToken ct)
        {
            var service = await repo.GetByIdAsync(request.Id, ct);
            if (service is null)
                throw new KeyNotFoundException("Service not found");

            // 1) Update names
            service.SetNames(request.Request.NameAr, request.Request.NameEn);

            if (request.Request.Stages is null || request.Request.Stages.Count == 0)
                throw new DomainException("Stages are required");

            foreach (var s in request.Request.Stages)
                _ = Stage.FromValue(s); 

            if (request.Request.Stages.GroupBy(x => x).Any(g => g.Count() > 1))
                throw new DomainException("Duplicated stage is not allowed");


            service.SetStages(request.Request.Stages);

            await uow.SaveChangesAsync(ct);
            return service.Id;
        }
    }
}
