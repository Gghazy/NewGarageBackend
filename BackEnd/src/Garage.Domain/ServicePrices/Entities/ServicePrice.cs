using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.ServicePrices.Entities;


public class ServicePrice : AggregateRoot
{
    public Guid ServiceId { get; private set; }
    public Guid MarkId { get; private set; }

    public int FromYear { get; private set; }
    public int ToYear { get; private set; }

    public decimal Price { get; private set; }


    private const int MinYear = 1950;
    private static readonly int MaxYear = DateTime.UtcNow.Year + 1;
    private const decimal MaxPrice = 1_000_000m;

    private ServicePrice() { }

    public ServicePrice(Guid serviceId, Guid markId, int fromYear, int toYear, decimal price)
    {
        Validate(serviceId, markId, fromYear, toYear, price);

        ServiceId = serviceId;
        MarkId = markId;
        FromYear = fromYear;
        ToYear = toYear;
        Price = price;
    }

    public void Update(int fromYear, int toYear, decimal price)
    {
        Validate(ServiceId, MarkId, fromYear, toYear, price);

        if (FromYear == fromYear && ToYear == toYear && Price == price)
            return;

        FromYear = fromYear;
        ToYear = toYear;
        Price = price;
    }

    private static void Validate(Guid serviceId, Guid markId, int fromYear, int toYear, decimal price)
    {
        if (serviceId == Guid.Empty)
            throw new DomainException("Service is required");

        if (markId == Guid.Empty)
            throw new DomainException("Car mark is required");

        if (fromYear > toYear)
            throw new DomainException("FromYear cannot be greater than ToYear");

        if (fromYear < MinYear || toYear > MaxYear)
            throw new DomainException($"Year must be between {MinYear} and {MaxYear}");

        if (price <= 0)
            throw new DomainException("Price must be greater than zero");

        if (price > MaxPrice)
            throw new DomainException("Price exceeds allowed limit");
    }
}
