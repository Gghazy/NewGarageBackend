namespace Garage.Application.Common.Exceptions;

public class ConflictException : DomainException
{
    public ConflictException(string code, string messageKey, string message = "")
        : base(code, messageKey, message)
    {
    }

    public ConflictException(string code, string messageKey, object? details)
        : base(code, messageKey, "", details)
    {
    }
}
