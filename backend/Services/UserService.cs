using backend.Models;

namespace backend.Services;

public class UserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserResult> DeleteUserAsync(string id, string currentUserId, bool isAdmin)
    {
        if (!isAdmin)
            return UserResult.Forbidden();

        if (id == currentUserId)
            return UserResult.Invalid("No puedes eliminar tu propio usuario");

        var target = await _repo.GetUserByIdAsync(id);
        if (target is null)
            return UserResult.NotFound();

        if (target.Role == "Admin")
        {
            var adminCount = await _repo.CountAdminsAsync();
            if (adminCount <= 1)
                return UserResult.Invalid("No se puede eliminar el último administrador");
        }

        if (!await _repo.DeleteUserAsync(id))
            return UserResult.NotFound();

        await _repo.DeleteUserAccessesAsync(id);
        return UserResult.Ok();
    }
}