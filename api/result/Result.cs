// api/results/Result.cs
namespace Orbital.Api.Results;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };

    // lets a method just `return someValue;` or `return someDto;` instead of wrapping every time
    public static implicit operator Result<T>(T value) => Success(value);
}