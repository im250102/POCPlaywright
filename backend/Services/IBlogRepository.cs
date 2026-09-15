using backend.Models;

namespace backend.Services;

public interface IBlogRepository
{
    Task<BlogPost?> GetPostByIdAsync(string id);
    Task<List<BlogPost>> GetPostsAsync(string? category = null);
    Task<BlogPost> CreatePostAsync(BlogPost post);
    Task<bool> UpdatePostAsync(BlogPost post);
    Task<bool> DeletePostAsync(string id);
    Task<List<BlogComment>> GetCommentsAsync(string postId);
    Task<bool> PostExistsAsync(string id);
    Task<BlogComment> AddCommentAsync(BlogComment comment);
}