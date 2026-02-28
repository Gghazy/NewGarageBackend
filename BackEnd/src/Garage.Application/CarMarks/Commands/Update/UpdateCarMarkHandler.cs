using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Manufacturers.Entity;
using MediatR;

namespace Garage.Application.CarMarks.Commands.Update;

public sealed class UpdateCarMarkHandler(
    ILookupRepository<CarMark> carMarkRepo,
    IReadRepository<Manufacturer> manufacturerRepo,
    IUnitOfWork uow)
    : IRequestHandler<UpdateCarMarkCommand, bool>
{
    public async Task<bool> Handle(UpdateCarMarkCommand command, CancellationToken ct)
    {
        var carMark = await carMarkRepo.GetByIdAsync(command.Id, ct);
        if (carMark is null) return false;

        var manufacturerExists = await manufacturerRepo.AnyAsync(m => m.Id == command.Request.ManufacturerId, ct);
        if (!manufacturerExists) return false;

        carMark.Update(command.Request.NameAr, command.Request.NameEn, command.Request.ManufacturerId);

        await uow.SaveChangesAsync(ct);
        return true;
    }
}
