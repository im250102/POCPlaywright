using backend.Models;
using backend.Services;
using NSubstitute;
using Xunit;

namespace backend.Tests;

public class DoctorServiceTests
{
    private readonly IDoctorRepository _repo = Substitute.For<IDoctorRepository>();
    private readonly DoctorService _service;

    public DoctorServiceTests()
    {
        _service = new DoctorService(_repo);
    }

    private static DoctorRequest ValidRequest(string? specialty = "cardiologia") => new()
    {
        Name = "  María  ",
        LastName = "  García López  ",
        Phone = "  +34 600 123 456  ",
        Specialty = specialty ?? "cardiologia"
    };

    private static Doctor SampleDoctor(string id = "doctor1") => new()
    {
        Id = id,
        UserId = "user1",
        Name = "María",
        LastName = "García López",
        Phone = "+34 600 123 456",
        Specialty = "cardiologia"
    };

    [Fact]
    public async Task CreateDoctorAsync_ConDatosValidos_CreaDoctorYNormaliza()
    {
        var result = await _service.CreateDoctorAsync(ValidRequest(), "user1");

        Assert.True(result.StatusCode == 201);
        Assert.NotNull(result.Value);
        Assert.Equal("user1", result.Value!.UserId);
        Assert.Equal("María", result.Value.Name);
        Assert.Equal("García López", result.Value.LastName);
        Assert.Equal("+34 600 123 456", result.Value.Phone);
        Assert.Equal("cardiologia", result.Value.Specialty);
        await _repo.Received(1).CreateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task CreateDoctorAsync_SinNombre_DevuelveError()
    {
        var request = ValidRequest();
        request.Name = "   ";

        var result = await _service.CreateDoctorAsync(request, "user1");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El nombre es obligatorio", result.Error);
        Assert.Null(result.Value);
        await _repo.DidNotReceive().CreateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task CreateDoctorAsync_SinApellidos_DevuelveError()
    {
        var request = ValidRequest();
        request.LastName = string.Empty;

        var result = await _service.CreateDoctorAsync(request, "user1");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Los apellidos son obligatorios", result.Error);
        Assert.Null(result.Value);
        await _repo.DidNotReceive().CreateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task CreateDoctorAsync_SinTelefono_DevuelveError()
    {
        var request = ValidRequest();
        request.Phone = "   ";

        var result = await _service.CreateDoctorAsync(request, "user1");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El teléfono es obligatorio", result.Error);
        Assert.Null(result.Value);
        await _repo.DidNotReceive().CreateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task CreateDoctorAsync_EspecialidadVacia_DevuelveError()
    {
        var request = ValidRequest();
        request.Specialty = string.Empty;

        var result = await _service.CreateDoctorAsync(request, "user1");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("La especialidad no es válida", result.Error);
        Assert.Null(result.Value);
        await _repo.DidNotReceive().CreateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task CreateDoctorAsync_EspecialidadInvalida_DevuelveError()
    {
        var result = await _service.CreateDoctorAsync(ValidRequest("oculistica"), "user1");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("La especialidad no es válida", result.Error);
        Assert.Null(result.Value);
        await _repo.DidNotReceive().CreateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task GetDoctorsAsync_DevuelveListaDeDoctores()
    {
        _repo.GetDoctorsAsync().Returns(new List<Doctor>
        {
            SampleDoctor("doctor1"),
            SampleDoctor("doctor2")
        });

        var result = await _service.GetDoctorsAsync();

        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task GetDoctorAsync_Existe_DevuelveDoctor()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns(SampleDoctor());

        var result = await _service.GetDoctorAsync("doctor1");

        Assert.Equal(200, result.StatusCode);
        Assert.Equal("doctor1", result.Value!.Id);
    }

    [Fact]
    public async Task GetDoctorAsync_NoExiste_DevuelveNotFound()
    {
        _repo.GetDoctorByIdAsync("desconocido").Returns((Doctor?)null);

        var result = await _service.GetDoctorAsync("desconocido");

        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task UpdateDoctorAsync_Existe_ActualizaDatos()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns(SampleDoctor());
        _repo.UpdateDoctorAsync(Arg.Any<Doctor>()).Returns(true);

        var result = await _service.UpdateDoctorAsync("doctor1", ValidRequest("dermatologia"));

        Assert.Equal(200, result.StatusCode);
        Assert.Equal("María", result.Value!.Name);
        Assert.Equal("dermatologia", result.Value.Specialty);
        await _repo.Received(1).UpdateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task UpdateDoctorAsync_NoExiste_DevuelveNotFound()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns((Doctor?)null);

        var result = await _service.UpdateDoctorAsync("doctor1", ValidRequest());

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().UpdateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task UpdateDoctorAsync_PayloadInvalido_DevuelveErrorYNoActualiza()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns(SampleDoctor());
        var request = ValidRequest();
        request.LastName = string.Empty;

        var result = await _service.UpdateDoctorAsync("doctor1", request);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Los apellidos son obligatorios", result.Error);
        await _repo.DidNotReceive().UpdateDoctorAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task DeleteDoctorAsync_Existe_Elimina()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns(SampleDoctor());
        _repo.DeleteDoctorAsync("doctor1").Returns(true);

        var result = await _service.DeleteDoctorAsync("doctor1");

        Assert.Equal(200, result.StatusCode);
        await _repo.Received(1).DeleteDoctorAsync("doctor1");
    }

    [Fact]
    public async Task DeleteDoctorAsync_NoExiste_DevuelveNotFound()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns((Doctor?)null);

        var result = await _service.DeleteDoctorAsync("doctor1");

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().DeleteDoctorAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeleteDoctorAsync_RepoNoElimina_DevuelveNotFound()
    {
        _repo.GetDoctorByIdAsync("doctor1").Returns(SampleDoctor());
        _repo.DeleteDoctorAsync("doctor1").Returns(false);

        var result = await _service.DeleteDoctorAsync("doctor1");

        Assert.Equal(404, result.StatusCode);
    }

    [Theory]
    [InlineData("medicina-general", true)]
    [InlineData("cardiologia", true)]
    [InlineData("dermatologia", true)]
    [InlineData("endocrinologia", true)]
    [InlineData("gastroenterologia", true)]
    [InlineData("geriatria", true)]
    [InlineData("hematologia", true)]
    [InlineData("infectologia", true)]
    [InlineData("nefrologia", true)]
    [InlineData("neumologia", true)]
    [InlineData("neurologia", true)]
    [InlineData("oncologia", true)]
    [InlineData("oftalmologia", true)]
    [InlineData("otorrinolaringologia", true)]
    [InlineData("pediatria", true)]
    [InlineData("psiquiatria", true)]
    [InlineData("reumatologia", true)]
    [InlineData("urologia", true)]
    [InlineData("ginecologia", true)]
    [InlineData("obstetricia", true)]
    [InlineData("traumatologia", true)]
    [InlineData("anestesiologia", true)]
    [InlineData("radiologia", true)]
    [InlineData("medicina-interna", true)]
    [InlineData("CARDIOLOGIA", true)]
    [InlineData("oculistica", false)]
    [InlineData("", false)]
    public void IsValidSpecialty_ValidaEspecialidadesPermitidas(string specialty, bool expected) =>
        Assert.Equal(expected, DoctorService.IsValidSpecialty(specialty));

    [Fact]
    public void AllowedSpecialties_ContieneTodasLasEspecialidadesMedicas()
    {
        Assert.Equal(24, DoctorService.AllowedSpecialties.Length);
        Assert.Contains("medicina-general", DoctorService.AllowedSpecialties);
        Assert.Contains("cardiologia", DoctorService.AllowedSpecialties);
        Assert.Contains("dermatologia", DoctorService.AllowedSpecialties);
        Assert.Contains("pediatria", DoctorService.AllowedSpecialties);
        Assert.Contains("medicina-interna", DoctorService.AllowedSpecialties);
    }
}