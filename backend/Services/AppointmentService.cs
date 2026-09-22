using backend.Models;

namespace backend.Services;

public class AppointmentService
{
    public AppointmentResult PrepareCreate(Appointment appointment)
    {
        if (string.IsNullOrWhiteSpace(appointment.PatientId) || string.IsNullOrWhiteSpace(appointment.PatientName))
            return AppointmentResult.Invalid("El paciente es obligatorio");

        if (string.IsNullOrWhiteSpace(appointment.DoctorId))
            return AppointmentResult.Invalid("El doctor es obligatorio");

        if (string.IsNullOrWhiteSpace(appointment.DoctorName))
            return AppointmentResult.Invalid("El doctor es obligatorio");

        if (appointment.Date == default)
            return AppointmentResult.Invalid("La fecha y hora son obligatorias");

        if (string.IsNullOrWhiteSpace(appointment.Reason))
            return AppointmentResult.Invalid("El motivo de la consulta es obligatorio");

        return AppointmentResult.Ok(appointment);
    }

    public string ResolveDoctorName(Appointment appointment, IReadOnlyDictionary<string, string> userByName)
    {
        if (!string.IsNullOrWhiteSpace(appointment.DoctorName))
            return appointment.DoctorName;

        if (!string.IsNullOrWhiteSpace(appointment.UserId) &&
            userByName.TryGetValue(appointment.UserId, out var creatorName) &&
            !string.IsNullOrWhiteSpace(creatorName))
            return creatorName;

        return "Sin asignar";
    }
}