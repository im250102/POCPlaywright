namespace backend.Services;

public class BlogResult
{
    public int StatusCode { get; init; } = 200;
    public string? Error { get; init; }

    public static BlogResult Ok() => new() { StatusCode = 200 };
    public static BlogResult Invalid(string error) => new() { StatusCode = 400, Error = error };
    public static BlogResult Forbidden() => new() { StatusCode = 403 };
    public static BlogResult NotFound() => new() { StatusCode = 404 };
}

public class BlogResult<T> : BlogResult
{
    public T? Value { get; init; }

    public static BlogResult<T> Ok(T value) => new() { StatusCode = 200, Value = value };
    public static BlogResult<T> Created(T value) => new() { StatusCode = 201, Value = value };
    public new static BlogResult<T> Invalid(string error) => new() { StatusCode = 400, Error = error };
    public new static BlogResult<T> Forbidden() => new() { StatusCode = 403 };
    public new static BlogResult<T> NotFound() => new() { StatusCode = 404 };
}