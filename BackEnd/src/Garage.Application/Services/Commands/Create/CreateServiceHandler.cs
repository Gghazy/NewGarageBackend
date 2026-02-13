using Garage.Application.Abstractions;
using Garage.Domain.Services.Entities;
using MediatR;


namespace Garage.Application.Services.Commands.Create
{
    public sealed class CreateServiceHandler(IRepository<Service> repo, IUnitOfWork uow)  : IRequestHandler<CreateServiceCommand, Guid>
    {

        public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken ct)
        {
            var service = new Service(request.Request.NameAr, request.Request.NameEn);

            service.SetStages(request.Request.Stages);

            await repo.AddAsync(service, ct);
            await uow.SaveChangesAsync(ct);

            return service.Id;
        }
    }
}