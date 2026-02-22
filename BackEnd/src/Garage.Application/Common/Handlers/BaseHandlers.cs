namespace Garage.Application.Common.Handlers;

using MediatR;

/// <summary>
/// Base class for command handlers that return a Result.
/// Provides consistent Result-based error handling pattern.
/// All command handlers should inherit from this class.
/// </summary>
/// <typeparam name="TRequest">The command request type</typeparam>
/// <typeparam name="TResponse">The response value type</typeparam>
public abstract class BaseCommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    /// <summary>
    /// Default error message for not found scenarios
    /// </summary>
    protected const string NotFoundError = "Resource not found";

    /// <summary>
    /// Default error message for conflict scenarios
    /// </summary>
    protected const string ConflictError = "Resource already exists";

    /// <summary>
    /// Handles the command and returns a Result.
    /// Implementation should focus on orchestration with Result-based error handling.
    /// All business logic should be delegated to domain entities and services.
    /// </summary>
    public abstract Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Protected helper to create an Ok result with the specified value
    /// </summary>
    protected Result<TResponse> Ok(TResponse value) => Result<TResponse>.Ok(value);

    /// <summary>
    /// Protected helper to create a Fail result with the specified error
    /// </summary>
    protected Result<TResponse> Fail(string error) => Result<TResponse>.Fail(error);
}

/// <summary>
/// Base class for query handlers.
/// All query handlers should inherit from this class.
/// </summary>
/// <typeparam name="TRequest">The query request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class BaseQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the query. Implementation should focus on data retrieval only.
    /// Query handlers must NOT modify data (read-only operations).
    /// </summary>
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
