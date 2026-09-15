using System.Security.Claims;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly BlogService _blog;

    public BlogController(BlogService blog)
    {
        _blog = blog;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var result = await _blog.GetPostsAsync(category);
        return Map(result, () => Ok(result.Value!
            .Select(p => ToDto(p, new List<BlogCommentDto>()))));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var post = await _blog.GetPostAsync(id);
        if (post.StatusCode == 404)
            return NotFound();

        var comments = await _blog.GetCommentsAsync(id);
        var commentDtos = (comments.Value ?? new List<BlogComment>())
            .Select(c => new BlogCommentDto
            {
                Id = c.Id ?? string.Empty,
                PostId = c.PostId,
                UserId = c.UserId,
                AuthorName = c.AuthorName,
                Text = c.Text,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return Map(post, () => Ok(ToDto(post.Value!, commentDtos)));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] BlogPostRequest request)
    {
        var result = await _blog.CreatePostAsync(request, GetUserId(), GetUserName());
        return Map(result, () => CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            ToDto(result.Value!, new List<BlogCommentDto>())));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, [FromBody] BlogPostRequest request)
    {
        var result = await _blog.UpdatePostAsync(id, request, GetUserId(), IsAdmin());
        return Map(result, () => Ok(ToDto(result.Value!, new List<BlogCommentDto>())));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _blog.DeletePostAsync(id, GetUserId(), IsAdmin());
        return Map(result, () => NoContent());
    }

    [HttpPost("{id}/comments")]
    [Authorize]
    public async Task<IActionResult> AddComment(string id, [FromBody] BlogCommentRequest request)
    {
        var result = await _blog.AddCommentAsync(id, request, GetUserId(), GetUserName());
        return Map(result, () => CreatedAtAction(
            nameof(GetById),
            new { id },
            new BlogCommentDto
            {
                Id = result.Value!.Id ?? string.Empty,
                PostId = result.Value.PostId,
                UserId = result.Value.UserId,
                AuthorName = result.Value.AuthorName,
                Text = result.Value.Text,
                CreatedAt = result.Value.CreatedAt
            }));
    }

    private static BlogPostDto ToDto(BlogPost post, List<BlogCommentDto> comments) => new()
    {
        Id = post.Id ?? string.Empty,
        UserId = post.UserId,
        AuthorName = post.AuthorName,
        Title = post.Title,
        Content = post.Content,
        Category = post.Category,
        CreatedAt = post.CreatedAt,
        UpdatedAt = post.UpdatedAt,
        Comments = comments
    };

    private IActionResult Map(BlogResult result, Func<IActionResult> onSuccess)
    {
        return result.StatusCode switch
        {
            400 => BadRequest(new { error = result.Error }),
            403 => Forbid(),
            404 => NotFound(),
            _ => onSuccess()
        };
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    private string GetUserName() =>
        User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario";

    private bool IsAdmin() =>
        (User.FindFirst("role")?.Value
            ?? User.FindFirst(ClaimTypes.Role)?.Value) == "Admin";
}