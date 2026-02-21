namespace Garage.Application.Common.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException()
        : base("Auth.Forbidden", "Auth.AccessDenied", "Access forbidden.")
    {
    }

    public ForbiddenException(string messageKey)
        : base("Auth.Forbidden", messageKey, "Access forbidden.")
    {
    }
}
