using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.Json;

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
                var traceId = context.TraceIdentifier;

                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

                var (status, code, messageKey, details) = MapException(ex);

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
        }

        private static (HttpStatusCode Status, string Code, string MessageKey, object? Details) MapException(Exception ex)
        {

            if (ex is UnauthorizedAccessException)
                return (HttpStatusCode.Unauthorized, "Auth.Unauthorized", "Auth.InvalidCredentials", null);

            if (ex is KeyNotFoundException)
                return (HttpStatusCode.NotFound, "NotFound", "NotFound", null);

            return (HttpStatusCode.InternalServerError, "Server.Error", "Server.Error", new
            {
                exception = ex.GetType().Name,
                ex.Message,
                ex.StackTrace
            });
        }
    }
}

