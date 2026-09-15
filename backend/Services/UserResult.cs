namespace backend.Services;

public class UserResult
{
    public int StatusCode { get; init; } = 200;
    public string? Error { get; init; }

    public static UserResult Ok() => new() { StatusCode = 200 };
    public static UserResult Invalid(string error) => new() { StatusCode = 400, Error = error };
    public static UserResult Forbidden() => new() { StatusCode = 403 };
    public static UserResult NotFound() => new() { StatusCode = 404 };
}