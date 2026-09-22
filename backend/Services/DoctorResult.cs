namespace backend.Services;

public class DoctorResult
{
    public int StatusCode { get; init; } = 200;
    public string? Error { get; init; }

    public static DoctorResult Ok() => new() { StatusCode = 200 };
    public static DoctorResult Invalid(string error) => new() { StatusCode = 400, Error = error };
    public static DoctorResult NotFound() => new() { StatusCode = 404 };
}

public class DoctorResult<T> : DoctorResult
{
    public T? Value { get; init; }

    public static DoctorResult<T> Ok(T value) => new() { StatusCode = 200, Value = value };
    public static DoctorResult<T> Created(T value) => new() { StatusCode = 201, Value = value };
    public new static DoctorResult<T> Invalid(string error) => new() { StatusCode = 400, Error = error };
    public new static DoctorResult<T> NotFound() => new() { StatusCode = 404 };
}