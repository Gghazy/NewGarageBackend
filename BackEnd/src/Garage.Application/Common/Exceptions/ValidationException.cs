namespace Garage.Application.Common.Exceptions;

public class ValidationException : DomainException
{
    private Dictionary<string, string[]> _errors = new();

    public ValidationException(string field, string messageKey)
        : base("Validation.Error", messageKey, "Validation failed.")
    {
        _errors[field] = new[] { messageKey };
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation.Error", "Validation.Error", "Validation failed.")
    {
        _errors = errors;
        Details = errors;
    }

    public ValidationException(string code, string messageKey, Dictionary<string, string[]> details)
        : base(code, messageKey, "Validation failed.", details)
    {
    }
}
