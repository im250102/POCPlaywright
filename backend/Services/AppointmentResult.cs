namespace backend.Services;

public class AppointmentResult
{
    public int StatusCode { get; init; } = 200;
    public string? Error { get; init; }
    public Models.Appointment? Value { get; init; }

    public static AppointmentResult Ok(Models.Appointment value) => new() { StatusCode = 200, Value = value };
    public static AppointmentResult Invalid(string error) => new() { StatusCode = 400, Error = error };
}