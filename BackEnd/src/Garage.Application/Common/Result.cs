namespace Garage.Application.Common;
public readonly struct Result<T>
{
    public bool Succeeded { get; }
    public string? Error { get; }
    public T? Value { get; }
    private Result(bool ok, T? value, string? error) { Succeeded = ok; Value = value; Error = error; }
    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Fail(string error) => new(false, default, error);
}



