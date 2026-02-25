using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetById;

public sealed class GetExaminationByIdQueryHandler(IReadRepository<Examination> repo)
    : BaseQueryHandler<GetExaminationByIdQuery, ExaminationDto?>
{
    public override async Task<ExaminationDto?> Handle(GetExaminationByIdQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Where(e => e.Id == request.Id)
            .Select(e => new ExaminationDto(
                // Id
                e.Id,
                // Status, Type
                e.Status.ToString(),
                e.Type.ToString(),
                // Client
                e.Client.ClientId,
                e.Client.NameAr,
                e.Client.NameEn,
                e.Client.PhoneNumber,
                // Branch
                e.Branch.BranchId,
                e.Branch.NameAr,
                e.Branch.NameEn,
                // Vehicle
                e.Vehicle.VehicleId,
                e.Vehicle.ManufacturerId,
                e.Vehicle.ManufacturerNameAr,
                e.Vehicle.ManufacturerNameEn,
                e.Vehicle.CarMarkId,
                e.Vehicle.CarMarkNameAr,
                e.Vehicle.CarMarkNameEn,
                e.Vehicle.Year,
                e.Vehicle.Color,
                e.Vehicle.Vin,
                e.Vehicle.HasPlate,
                e.Vehicle.HasPlate ? e.Vehicle.Plate!.Letters : null,
                e.Vehicle.HasPlate ? e.Vehicle.Plate!.Numbers : null,
                e.Vehicle.Mileage,
                e.Vehicle.MileageUnit.ToString(),
                e.Vehicle.Transmission != null ? e.Vehicle.Transmission.ToString() : null,
                // Examination meta
                e.HasWarranty,
                e.HasPhotos,
                e.MarketerCode,
                e.Notes,
                // Financials
                e.TotalPrice.Amount,
                e.TaxRate,
                e.TaxAmount.Amount,
                e.TotalWithTax.Amount,
                e.TotalPrice.Currency,
                e.Payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount),
                e.Payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount),
                e.TotalWithTax.Amount
                    - e.Payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount)
                    + e.Payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount),
                // Items
                e.Items.Select(i => new ExaminationItemDto(
                    i.Id,
                    i.Service.ServiceId,
                    i.Service.NameAr,
                    i.Service.NameEn,
                    i.Price.Amount,
                    i.Price.Currency,
                    i.Status.ToString(),
                    i.Notes
                )).ToList(),
                // Payments
                e.Payments.Select(p => new PaymentDto(
                    p.Id,
                    p.Amount.Amount,
                    p.Amount.Currency,
                    p.Method.ToString(),
                    p.Type.ToString(),
                    p.Notes,
                    p.CreatedAtUtc
                )).ToList(),
                // CreatedAt
                e.CreatedAtUtc
            ))
            .FirstOrDefaultAsync(ct);
    }
}
