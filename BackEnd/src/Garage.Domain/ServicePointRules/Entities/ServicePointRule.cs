using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;

namespace Garage.Domain.ServicePointRules.Entities;

public class ServicePointRule : AggregateRoot
{
    public decimal FromAmount { get; private set; }
    public decimal ToAmount { get; private set; }
    public int Points { get; private set; }
    public bool IsActive { get; private set; }

    private ServicePointRule() { }

    public ServicePointRule(decimal fromAmount, decimal toAmount, int points, bool isActive)
    {
        Validate(fromAmount, toAmount, points);
        FromAmount = fromAmount;
        ToAmount = toAmount;
        Points = points;
        IsActive = isActive;
    }

    public void Update(decimal fromAmount, decimal toAmount, int points, bool isActive)
    {
        Validate(fromAmount, toAmount, points);
        FromAmount = fromAmount;
        ToAmount = toAmount;
        Points = points;
        IsActive = isActive;
    }

    public bool MatchesAmount(decimal invoiceTotal)
        => IsActive && !IsDeleted && invoiceTotal >= FromAmount && invoiceTotal <= ToAmount;

    private static void Validate(decimal fromAmount, decimal toAmount, int points)
    {
        if (fromAmount < 0)
            throw new DomainException("From amount cannot be negative");
        if (toAmount <= fromAmount)
            throw new DomainException("To amount must be greater than from amount");
        if (points <= 0)
            throw new DomainException("Points must be greater than zero");
    }
}
