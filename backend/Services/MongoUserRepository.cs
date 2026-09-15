using backend.Models;
using MongoDB.Driver;

namespace backend.Services;

public class MongoUserRepository : IUserRepository
{
    private readonly MongoDbContext _db;

    public MongoUserRepository(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserByIdAsync(string id) =>
        await _db.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<long> CountAdminsAsync() =>
        await _db.Users.CountDocumentsAsync(u => u.Role == "Admin");

    public async Task<bool> DeleteUserAsync(string id)
    {
        var deleted = await _db.Users.DeleteOneAsync(u => u.Id == id);
        return deleted.IsAcknowledged && deleted.DeletedCount > 0;
    }

    public async Task DeleteUserAccessesAsync(string userId) =>
        await _db.UserAccesses.DeleteManyAsync(a => a.UserId == userId);
}