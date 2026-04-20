using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ServiceDiscounts.Entities;

public class ServiceDiscount : AggregateRoot
{
    public Guid ServiceId { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    private const decimal MinPercent = 0.01m;
    private const decimal MaxPercent = 100m;

    private ServiceDiscount() { }

    public ServiceDiscount(Guid serviceId, decimal discountPercent, DateTime startDate, DateTime endDate, bool isActive)
    {
        Validate(serviceId, discountPercent, startDate, endDate);

        ServiceId = serviceId;
        DiscountPercent = discountPercent;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }

    public void Update(decimal discountPercent, DateTime startDate, DateTime endDate, bool isActive)
    {
        Validate(ServiceId, discountPercent, startDate, endDate);

        if (DiscountPercent == discountPercent && StartDate == startDate && EndDate == endDate && IsActive == isActive)
            return;

        DiscountPercent = discountPercent;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }

    public bool IsEffectiveOn(DateTime date)
        => IsActive && !IsDeleted && date >= StartDate && date <= EndDate;

    private static void Validate(Guid serviceId, decimal discountPercent, DateTime startDate, DateTime endDate)
    {
        if (serviceId == Guid.Empty)
            throw new DomainException("Service is required");

        if (discountPercent < MinPercent || discountPercent > MaxPercent)
            throw new DomainException($"Discount percent must be between {MinPercent}% and {MaxPercent}%");

        if (startDate > endDate)
            throw new DomainException("Start date cannot be after end date");
    }
}
