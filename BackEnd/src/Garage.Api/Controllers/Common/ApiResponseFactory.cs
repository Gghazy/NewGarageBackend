namespace Garage.Api.Controllers.Common;

using Garage.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

/// <summary>
/// Factory methods for creating unified API responses
/// </summary>
public static class ApiResponseFactory
{
    /// <summary>
    /// Creates an appropriate HTTP response for a domain exception
    /// </summary>
    public static IActionResult FromException(DomainException ex, IStringLocalizer localizer, ControllerBase controller)
    {
        var message = localizer[ex.MessageKey]!;

        return ex switch
        {
            NotFoundException => controller.NotFound(new { code = ex.Code, message }),
            ConflictException => controller.Conflict(new { code = ex.Code, message }),
            ValidationException => controller.BadRequest(new { code = ex.Code, message, details = ex.Details }),
            UnauthorizedException => controller.Unauthorized(new { code = ex.Code, message }),
            ForbiddenException => controller.StatusCode(StatusCodes.Status403Forbidden, new { code = ex.Code, message }),
            _ => controller.BadRequest(new { code = ex.Code, message })
        };
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    public static object CreateValidationError(Dictionary<string, string[]> errors, string code = "Validation.Error")
    {
        return new
        {
            code,
            errors
        };
    }
}
