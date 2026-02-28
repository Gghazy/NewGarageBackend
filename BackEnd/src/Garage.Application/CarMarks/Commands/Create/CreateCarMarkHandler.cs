using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Manufacturers.Entity;

namespace Garage.Application.CarMarks.Commands.Create;

public sealed class CreateCarMarkHandler(
    ILookupRepository<CarMark> carMarkRepo,
    IReadRepository<Manufacturer> manufacturerRepo,
    IUnitOfWork uow)
    : BaseCommandHandler<CreateCarMarkCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateCarMarkCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var manufacturerExists = await manufacturerRepo.AnyAsync(m => m.Id == req.ManufacturerId, ct);
        if (!manufacturerExists)
            return Fail("Manufacturer.NotFound");

        var carMark = new CarMark(req.NameAr, req.NameEn, req.ManufacturerId);

        await carMarkRepo.AddAsync(carMark, ct);
        await uow.SaveChangesAsync(ct);

        return Ok(carMark.Id);
    }
}
