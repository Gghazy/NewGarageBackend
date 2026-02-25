using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Common;
using Garage.Contracts.Examinations;

namespace Garage.Application.Examinations.Queries.GetAll;

public sealed class GetAllExaminationsQueryHandler(IReadRepository<Examination> repo)
    : BaseQueryHandler<GetAllExaminationsQuery, QueryResult<ExaminationDto>>
{
    public override async Task<QueryResult<ExaminationDto>> Handle(GetAllExaminationsQuery request, CancellationToken ct)
    {
        var search = request.Search;

        var query = repo.Query()
            .WhereIf(
                !string.IsNullOrWhiteSpace(search.TextSearch),
                e => e.Client.NameAr.Contains(search.TextSearch!)   ||
                     e.Client.NameEn.Contains(search.TextSearch!)   ||
                     e.Client.PhoneNumber.Contains(search.TextSearch!) ||
                     (e.InvoiceNumber != null && e.InvoiceNumber.Contains(search.TextSearch!)) ||
                     (e.MarketerCode != null && e.MarketerCode.Contains(search.TextSearch!)))
            .Select(e => new ExaminationDto(
                // Id
                e.Id,
                e.InvoiceNumber,
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
            ));

        return await query.ToQueryResult(
            search.CurrentPage,
            search.ItemsPerPage,
            ct:         ct);
    }
}
