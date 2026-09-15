using backend.Models;

namespace backend.Services;

public class BlogService
{
    public static readonly string[] AllowedCategories =
        { "General", "Tecnologia", "Personal", "Tutorial" };

    private readonly IBlogRepository _repo;

    public BlogService(IBlogRepository repo)
    {
        _repo = repo;
    }

    public async Task<BlogResult<List<BlogPost>>> GetPostsAsync(string? category)
    {
        if (!string.IsNullOrWhiteSpace(category) && !IsValidCategory(category))
            return BlogResult<List<BlogPost>>.Invalid("La categoría no es válida");

        var posts = await _repo.GetPostsAsync(category);
        return BlogResult<List<BlogPost>>.Ok(posts);
    }

    public async Task<BlogResult<BlogPost>> GetPostAsync(string id)
    {
        var post = await _repo.GetPostByIdAsync(id);
        if (post is null)
            return BlogResult<BlogPost>.NotFound();

        return BlogResult<BlogPost>.Ok(post);
    }

    public async Task<BlogResult<BlogPost>> CreatePostAsync(BlogPostRequest request, string userId, string authorName)
    {
        var (valid, error) = ValidatePost(request);
        if (!valid)
            return BlogResult<BlogPost>.Invalid(error);

        var post = new BlogPost
        {
            UserId = userId,
            AuthorName = authorName,
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            Category = NormalizeCategory(request.Category),
            CreatedAt = DateTime.UtcNow
        };

        await _repo.CreatePostAsync(post);
        return BlogResult<BlogPost>.Created(post);
    }

    public async Task<BlogResult<BlogPost>> UpdatePostAsync(
        string id, BlogPostRequest request, string userId, bool isAdmin)
    {
        var existing = await _repo.GetPostByIdAsync(id);
        if (existing is null)
            return BlogResult<BlogPost>.NotFound();

        if (existing.UserId != userId && !isAdmin)
            return BlogResult<BlogPost>.Forbidden();

        var (valid, error) = ValidatePost(request);
        if (!valid)
            return BlogResult<BlogPost>.Invalid(error);

        existing.Title = request.Title.Trim();
        existing.Content = request.Content.Trim();
        existing.Category = NormalizeCategory(request.Category);
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdatePostAsync(existing);
        return BlogResult<BlogPost>.Ok(existing);
    }

    public async Task<BlogResult> DeletePostAsync(string id, string userId, bool isAdmin)
    {
        var existing = await _repo.GetPostByIdAsync(id);
        if (existing is null)
            return BlogResult.NotFound();

        if (existing.UserId != userId && !isAdmin)
            return BlogResult.Forbidden();

        var deleted = await _repo.DeletePostAsync(id);
        return deleted
            ? BlogResult.Ok()
            : BlogResult.NotFound();
    }

    public async Task<BlogResult<List<BlogComment>>> GetCommentsAsync(string postId)
    {
        var exists = await _repo.PostExistsAsync(postId);
        if (!exists)
            return BlogResult<List<BlogComment>>.NotFound();

        var comments = await _repo.GetCommentsAsync(postId);
        return BlogResult<List<BlogComment>>.Ok(comments);
    }

    public async Task<BlogResult<BlogComment>> AddCommentAsync(
        string postId, BlogCommentRequest request, string userId, string authorName)
    {
        var exists = await _repo.PostExistsAsync(postId);
        if (!exists)
            return BlogResult<BlogComment>.NotFound();

        var (valid, error) = ValidateComment(request.Text);
        if (!valid)
            return BlogResult<BlogComment>.Invalid(error);

        var comment = new BlogComment
        {
            PostId = postId,
            UserId = userId,
            AuthorName = authorName,
            Text = request.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddCommentAsync(comment);
        return BlogResult<BlogComment>.Created(comment);
    }

    public (bool Valid, string Error) ValidatePost(BlogPostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return (false, "El título es obligatorio");

        if (string.IsNullOrWhiteSpace(request.Content))
            return (false, "El contenido es obligatorio");

        if (!IsValidCategory(request.Category))
            return (false, "La categoría no es válida");

        return (true, string.Empty);
    }

    public (bool Valid, string Error) ValidateComment(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return (false, "El comentario no puede estar vacío");

        return (true, string.Empty);
    }

    public static bool IsValidCategory(string category) =>
        AllowedCategories.Any(c => string.Equals(c, category, StringComparison.OrdinalIgnoreCase));

    public static string NormalizeCategory(string category) =>
        AllowedCategories.First(c => string.Equals(c, category, StringComparison.OrdinalIgnoreCase));
}