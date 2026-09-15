using backend.Models;

namespace backend.Services;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    Task<long> CountAdminsAsync();
    Task<bool> DeleteUserAsync(string id);
    Task DeleteUserAccessesAsync(string userId);
}