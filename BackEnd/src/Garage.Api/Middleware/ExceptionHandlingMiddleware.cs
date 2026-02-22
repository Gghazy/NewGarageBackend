using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.Json;
using Garage.Application.Common.Exceptions;

namespace Garage.Api.Middleware
{
    public sealed record ApiError(
        string Code,
        string Message,
        string? TraceId = null,
        object? Details = null
    );

    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private readonly IStringLocalizer _T;

        public ExceptionHandlingMiddleware(
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env,
            IStringLocalizer T)
        {
            _logger = logger;
            _env = env;
            _T = T;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var (status, code, messageKey, details) = MapException(exception);

            // Log based on severity
            if (status == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled server error. TraceId={TraceId}", traceId);
            }
            else
            {
                _logger.LogWarning(exception, "Client error occurred. Code={Code}, TraceId={TraceId}", code, traceId);
            }

            var payload = new ApiError(
                Code: code,
                Message: _T[messageKey],
                TraceId: traceId,
                Details: _env.IsDevelopment() ? details : null
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private (HttpStatusCode Status, string Code, string MessageKey, object? Details) MapException(Exception ex)
        {
            // Domain-specific exceptions with localization support
            if (ex is DomainException domainEx)
            {
                var status = GetStatusCodeForDomainException(domainEx);
                return (status, domainEx.Code, domainEx.MessageKey, domainEx.Details);
            }

            // Built-in exceptions mapping
            if (ex is UnauthorizedAccessException)
                return (HttpStatusCode.Unauthorized, "Auth.Unauthorized", "Auth.Unauthorized", null);

            if (ex is KeyNotFoundException)
                return (HttpStatusCode.NotFound, "NotFound", "Common.NotFound", null);

            if (ex is ArgumentException)
                return (HttpStatusCode.BadRequest, "Validation.Error", "Validation.BadArgument", null);

            if (ex is InvalidOperationException)
                return (HttpStatusCode.BadRequest, "Operation.Invalid", "Operation.InvalidOperation", null);

            // Default internal server error
            return (HttpStatusCode.InternalServerError, "Server.Error", "Server.Error", new
            {
                exception = ex.GetType().Name,
                message = ex.Message,
                stackTrace = ex.StackTrace
            });
        }

        private HttpStatusCode GetStatusCodeForDomainException(DomainException ex)
        {
            return ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                ConflictException => HttpStatusCode.Conflict,
                ValidationException => HttpStatusCode.BadRequest,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                ForbiddenException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.BadRequest
            };
        }
    }
}

