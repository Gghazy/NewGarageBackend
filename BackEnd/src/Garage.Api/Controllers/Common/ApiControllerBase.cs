namespace Garage.Api.Controllers.Common;

using Garage.Application.Common;
using Garage.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

/// <summary>
/// Base controller class providing unified response handling and error marshalling.
/// All controllers should inherit from this class.
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// String localizer for translating messages
    /// </summary>
    protected readonly IStringLocalizer _localizer;

    protected ApiControllerBase(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    /// <summary>
    /// Returns a successful response with data
    /// </summary>
    protected IActionResult Success<T>(T data, string? message = null)
    {
        return Ok(new ApiResponse<T>(data, message));
    }

    /// <summary>
    /// Returns a successful response with a localized message
    /// </summary>
    protected IActionResult SuccessMessage(string messageKey)
    {
        return Ok(new ApiMessage(_localizer[messageKey]!));
    }

    /// <summary>
    /// Handles a Result from MediatR, converting to appropriate HTTP response
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.Succeeded)
            return Success(result.Value);

        return BadRequest(new ApiMessage(_localizer[result.Error!]));
    }

    /// <summary>
    /// Handles a Result with custom success message
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result, string successMessageKey)
    {
        if (result.Succeeded)
            return SuccessMessage(successMessageKey);

        return BadRequest(new ApiMessage(_localizer[result.Error!]));
    }

    /// <summary>
    /// Returns a not found response
    /// </summary>
    protected IActionResult NotFoundMessage(string messageKey)
    {
        return NotFound(new ApiMessage(_localizer[messageKey]!));
    }

    /// <summary>
    /// Returns a conflict response (409)
    /// </summary>
    protected IActionResult ConflictMessage(string messageKey)
    {
        return Conflict(new ApiMessage(_localizer[messageKey]!));
    }

    /// <summary>
    /// Returns an unauthorized response (401)
    /// </summary>
    protected IActionResult UnauthorizedMessage(string messageKey)
    {
        return Unauthorized(new ApiMessage(_localizer[messageKey]!));
    }

    /// <summary>
    /// Returns a forbidden response (403)
    /// </summary>
    protected IActionResult ForbiddenMessage(string messageKey)
    {
        return StatusCode(StatusCodes.Status403Forbidden, new ApiMessage(_localizer[messageKey]!));
    }

    /// <summary>
    /// Returns a bad request response
    /// </summary>
    protected IActionResult BadRequestMessage(string messageKey)
    {
        return BadRequest(new ApiMessage(_localizer[messageKey]!));
    }

    /// <summary>
    /// Returns a successful response with data and localized message
    /// </summary>
    protected IActionResult SuccessWithMessage<T>(T data, string messageKey)
    {
        return Ok(new ApiResponse<T>(data, _localizer[messageKey]!));
    }
}
