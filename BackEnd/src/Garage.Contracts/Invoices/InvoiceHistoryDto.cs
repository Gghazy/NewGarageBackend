namespace Garage.Contracts.Invoices;

public sealed record InvoiceHistoryDto(
    Guid Id,
    string Action,
    string? Details,
    Guid? PerformedById,
    string? PerformedByNameAr,
    string? PerformedByNameEn,
    DateTime CreatedAtUtc
);
