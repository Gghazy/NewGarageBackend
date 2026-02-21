namespace Garage.Application.Common.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string resourceName)
        : base("NotFound", "Common.NotFound", $"{resourceName} not found.")
    {
    }

    public NotFoundException(string code, string messageKey)
        : base(code, messageKey, messageKey)
    {
    }
}
