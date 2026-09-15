using backend.Models;
using backend.Services;
using NSubstitute;
using Xunit;

namespace backend.Tests;

public class UserServiceTests
{
    private readonly IUserRepository _repo = Substitute.For<IUserRepository>();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_repo);
    }

    private static User SampleUser(string id = "user1", string role = "Medico") => new()
    {
        Id = id,
        Name = "Usuario de prueba",
        Email = "usuario@test.com",
        PasswordHash = "hash",
        Role = role,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task DeleteUserAsync_NoEsAdmin_DevuelveForbidden()
    {
        var result = await _service.DeleteUserAsync("user1", "me", isAdmin: false);

        Assert.Equal(403, result.StatusCode);
        await _repo.DidNotReceive().DeleteUserAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeleteUserAsync_SeEliminaASiMismo_DevuelveError()
    {
        var result = await _service.DeleteUserAsync("me", "me", isAdmin: true);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("No puedes eliminar tu propio usuario", result.Error);
        await _repo.DidNotReceive().DeleteUserAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeleteUserAsync_UsuarioNoExiste_DevuelveNotFound()
    {
        _repo.GetUserByIdAsync("desconocido").Returns((User?)null);

        var result = await _service.DeleteUserAsync("desconocido", "me", isAdmin: true);

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().DeleteUserAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeleteUserAsync_EsElUnicoAdmin_DevuelveError()
    {
        _repo.GetUserByIdAsync("admin1").Returns(SampleUser("admin1", "Admin"));
        _repo.CountAdminsAsync().Returns(1);

        var result = await _service.DeleteUserAsync("admin1", "me", isAdmin: true);

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("No se puede eliminar el último administrador", result.Error);
        await _repo.DidNotReceive().DeleteUserAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeleteUserAsync_EliminaMedico_ActualizaAccesos()
    {
        _repo.GetUserByIdAsync("user1").Returns(SampleUser("user1", "Medico"));
        _repo.DeleteUserAsync("user1").Returns(true);

        var result = await _service.DeleteUserAsync("user1", "me", isAdmin: true);

        Assert.Equal(200, result.StatusCode);
        await _repo.Received(1).DeleteUserAsync("user1");
        await _repo.Received(1).DeleteUserAccessesAsync("user1");
    }

    [Fact]
    public async Task DeleteUserAsync_AdminConMasDeUnAdmin_Elimina()
    {
        _repo.GetUserByIdAsync("admin1").Returns(SampleUser("admin1", "Admin"));
        _repo.CountAdminsAsync().Returns(2);
        _repo.DeleteUserAsync("admin1").Returns(true);

        var result = await _service.DeleteUserAsync("admin1", "me", isAdmin: true);

        Assert.Equal(200, result.StatusCode);
        await _repo.Received(1).DeleteUserAsync("admin1");
        await _repo.Received(1).DeleteUserAccessesAsync("admin1");
    }

    [Fact]
    public async Task DeleteUserAsync_RepoNoElimina_DevuelveNotFound()
    {
        _repo.GetUserByIdAsync("user1").Returns(SampleUser("user1", "Medico"));
        _repo.DeleteUserAsync("user1").Returns(false);

        var result = await _service.DeleteUserAsync("user1", "me", isAdmin: true);

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().DeleteUserAccessesAsync(Arg.Any<string>());
    }
}