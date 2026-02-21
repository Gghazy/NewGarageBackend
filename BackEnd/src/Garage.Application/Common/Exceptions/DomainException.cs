namespace Garage.Application.Common.Exceptions;

/// <summary>
/// Base exception for domain-specific errors that should be localized
/// </summary>
public class DomainException : Exception
{
    public string Code { get; set; }
    public string MessageKey { get; set; }
    public object? Details { get; set; }

    public DomainException(string code, string messageKey, string message = "", object? details = null)
        : base(message)
    {
        Code = code;
        MessageKey = messageKey;
        Details = details;
    }
}
