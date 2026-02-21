namespace Garage.Application.Common.Extensions;

using Garage.Application.Common.Exceptions;

/// <summary>
/// Extension methods for Result<T> to provide fluent error handling and transformation
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a Result to a different type while preserving success/failure state
    /// </summary>
    public static Result<TNew> Map<T, TNew>(this Result<T> result, Func<T, TNew> mapper)
    {
        return result.Succeeded
            ? Result<TNew>.Ok(mapper(result.Value!))
            : Result<TNew>.Fail(result.Error!);
    }

    /// <summary>
    /// Executes a function on the value if successful
    /// </summary>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.Succeeded)
            action(result.Value!);
        return result;
    }

    /// <summary>
    /// Executes a function if the result failed
    /// </summary>
    public static Result<T> TapError<T>(this Result<T> result, Action<string> action)
    {
        if (!result.Succeeded)
            action(result.Error!);
        return result;
    }

    /// <summary>
    /// Binds/flatmaps a Result to another Result
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<Result<TNew>>> binder)
    {
        return result.Succeeded
            ? await binder(result.Value!)
            : Result<TNew>.Fail(result.Error!);
    }

    /// <summary>
    /// Converts a Result to a domain exception if failed
    /// </summary>
    public static T GetValueOrThrow<T>(this Result<T> result)
    {
        if (!result.Succeeded)
            throw new DomainException("Operation.Invalid", "Operation.OperationFailed", result.Error);

        return result.Value!;
    }

    /// <summary>
    /// Gets the value or returns a default value
    /// </summary>
    public static T GetValueOrDefault<T>(this Result<T> result, T defaultValue = default!)
    {
        return result.Succeeded ? result.Value! : defaultValue;
    }

    /// <summary>
    /// Throws DomainException if result failed
    /// </summary>
    public static void ThrowIfFailed<T>(this Result<T> result, string code = "Operation.Invalid", string messageKey = "Operation.OperationFailed")
    {
        if (!result.Succeeded)
            throw new DomainException(code, messageKey, result.Error);
    }

    /// <summary>
    /// Deconstructs Result for pattern matching
    /// </summary>
    public static void Deconstruct<T>(this Result<T> result, out bool succeeded, out T? value, out string? error)
    {
        succeeded = result.Succeeded;
        value = result.Value;
        error = result.Error;
    }

    /// <summary>
    /// Matches on the Result (success/failure handling)
    /// </summary>
    public static TResult Match<T, TResult>(
        this Result<T> result,
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return result.Succeeded
            ? onSuccess(result.Value!)
            : onFailure(result.Error!);
    }

    /// <summary>
    /// Matches on the Result with async operations
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Result<T> result,
        Func<T, Task<TResult>> onSuccess,
        Func<string, Task<TResult>> onFailure)
    {
        return result.Succeeded
            ? await onSuccess(result.Value!)
            : await onFailure(result.Error!);
    }
}
