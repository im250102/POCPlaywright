using backend.Models;
using backend.Services;
using Xunit;

namespace backend.Tests;

public class AppointmentServiceTests
{
    private readonly AppointmentService _service = new();

    private static Appointment ValidAppointment() => new()
    {
        UserId = "user1",
        PatientId = "patient1",
        PatientName = "Ana Pérez",
        DoctorId = "doctor1",
        DoctorName = "María García López",
        Date = new DateTime(2026, 8, 20, 10, 30, 0),
        Reason = "Dolor de cabeza",
        Status = "Scheduled"
    };

    [Fact]
    public void PrepareCreate_ConDatosValidosYDoctor_DevuelveCita()
    {
        var result = _service.PrepareCreate(ValidAppointment());

        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("doctor1", result.Value!.DoctorId);
        Assert.Equal("María García López", result.Value.DoctorName);
    }

    [Fact]
    public void PrepareCreate_SinPaciente_DevuelveError()
    {
        var appointment = ValidAppointment();
        appointment.PatientId = "";

        var result = _service.PrepareCreate(appointment);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El paciente es obligatorio", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void PrepareCreate_SinNombreDePaciente_DevuelveError()
    {
        var appointment = ValidAppointment();
        appointment.PatientName = "   ";

        var result = _service.PrepareCreate(appointment);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El paciente es obligatorio", result.Error);
    }

    [Fact]
    public void PrepareCreate_SinDoctor_DevuelveError()
    {
        var appointment = ValidAppointment();
        appointment.DoctorId = string.Empty;

        var result = _service.PrepareCreate(appointment);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El doctor es obligatorio", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void PrepareCreate_SinNombreDeDoctor_DevuelveError()
    {
        var appointment = ValidAppointment();
        appointment.DoctorName = " ";

        var result = _service.PrepareCreate(appointment);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El doctor es obligatorio", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void PrepareCreate_SinFecha_DevuelveError()
    {
        var appointment = ValidAppointment();
        appointment.Date = default;

        var result = _service.PrepareCreate(appointment);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("La fecha y hora son obligatorias", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void PrepareCreate_SinMotivo_DevuelveError()
    {
        var appointment = ValidAppointment();
        appointment.Reason = string.Empty;

        var result = _service.PrepareCreate(appointment);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El motivo de la consulta es obligatorio", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ResolveDoctorName_CitaConDoctor_UsaNombreDelDoctor()
    {
        var appointment = ValidAppointment();

        var doctorName = _service.ResolveDoctorName(appointment, new Dictionary<string, string>());

        Assert.Equal("María García López", doctorName);
    }

    [Fact]
    public void ResolveDoctorName_CitaSinDoctorNombre_UsaUsuarioCreador()
    {
        var appointment = ValidAppointment();
        appointment.DoctorName = string.Empty;

        var doctorName = _service.ResolveDoctorName(
            appointment,
            new Dictionary<string, string> { ["user1"] = "Dr. González" });

        Assert.Equal("Dr. González", doctorName);
    }

    [Fact]
    public void ResolveDoctorName_SinDoctorNiUsuario_DevuelveSinAsignar()
    {
        var appointment = ValidAppointment();
        appointment.DoctorName = string.Empty;
        appointment.UserId = string.Empty;

        var doctorName = _service.ResolveDoctorName(appointment, new Dictionary<string, string>());

        Assert.Equal("Sin asignar", doctorName);
    }
}