using backend.Models;
using MongoDB.Driver;

namespace backend.Services;

public class MongoBlogRepository : IBlogRepository
{
    private readonly MongoDbContext _db;

    public MongoBlogRepository(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<BlogPost?> GetPostByIdAsync(string id) =>
        await _db.BlogPosts.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<List<BlogPost>> GetPostsAsync(string? category = null)
    {
        var filter = string.IsNullOrWhiteSpace(category)
            ? Builders<BlogPost>.Filter.Empty
            : Builders<BlogPost>.Filter.Eq(p => p.Category, category);

        return await _db.BlogPosts
            .Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<BlogPost> CreatePostAsync(BlogPost post)
    {
        await _db.BlogPosts.InsertOneAsync(post);
        return post;
    }

    public async Task<bool> UpdatePostAsync(BlogPost post)
    {
        var result = await _db.BlogPosts.ReplaceOneAsync(p => p.Id == post.Id, post);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeletePostAsync(string id)
    {
        var deleted = await _db.BlogPosts.DeleteOneAsync(p => p.Id == id);
        if (deleted.IsAcknowledged)
        {
            await _db.BlogComments.DeleteManyAsync(c => c.PostId == id);
        }

        return deleted.IsAcknowledged && deleted.DeletedCount > 0;
    }

    public async Task<List<BlogComment>> GetCommentsAsync(string postId) =>
        await _db.BlogComments
            .Find(c => c.PostId == postId)
            .SortBy(c => c.CreatedAt)
            .ToListAsync();

    public async Task<bool> PostExistsAsync(string id) =>
        await _db.BlogPosts.Find(p => p.Id == id).AnyAsync();

    public async Task<BlogComment> AddCommentAsync(BlogComment comment)
    {
        await _db.BlogComments.InsertOneAsync(comment);
        return comment;
    }
}