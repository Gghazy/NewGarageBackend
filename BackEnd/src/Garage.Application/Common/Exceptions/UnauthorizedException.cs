namespace Garage.Application.Common.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException()
        : base("Auth.Unauthorized", "Auth.Unauthorized", "Unauthorized access.")
    {
    }

    public UnauthorizedException(string messageKey)
        : base("Auth.Unauthorized", messageKey, "Unauthorized access.")
    {
    }
}
