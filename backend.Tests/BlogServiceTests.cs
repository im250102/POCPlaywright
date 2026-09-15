using backend.Models;
using backend.Services;
using NSubstitute;
using Xunit;

namespace backend.Tests;

public class BlogServiceTests
{
    private readonly IBlogRepository _repo = Substitute.For<IBlogRepository>();
    private readonly BlogService _service;

    public BlogServiceTests()
    {
        _service = new BlogService(_repo);
    }

    private static BlogPostRequest ValidRequest(string? category = "Tecnologia") => new()
    {
        Title = "  Mi primer post  ",
        Content = "  Contenido del post  ",
        Category = category ?? "Tecnologia"
    };

    private static BlogPost SamplePost(string id = "post1", string userId = "user1") => new()
    {
        Id = id,
        UserId = userId,
        AuthorName = "Autor",
        Title = "Titulo",
        Content = "Contenido",
        Category = "General",
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task CreatePostAsync_ConDatosValidos_CreaPostYNormaliza()
    {
        var result = await _service.CreatePostAsync(ValidRequest("tecnologia"), "user1", "Autor");

        Assert.True(result.StatusCode == 201);
        Assert.NotNull(result.Value);
        Assert.Equal("user1", result.Value!.UserId);
        Assert.Equal("Autor", result.Value.AuthorName);
        Assert.Equal("Mi primer post", result.Value.Title);
        Assert.Equal("Contenido del post", result.Value.Content);
        Assert.Equal("Tecnologia", result.Value.Category);
        await _repo.Received(1).CreatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task CreatePostAsync_SinTitulo_DevuelveError()
    {
        var request = ValidRequest();
        request.Title = "   ";

        var result = await _service.CreatePostAsync(request, "user1", "Autor");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El título es obligatorio", result.Error);
        Assert.Null(result.Value);
        await _repo.DidNotReceive().CreatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task CreatePostAsync_SinContenido_DevuelveError()
    {
        var request = ValidRequest();
        request.Content = string.Empty;

        var result = await _service.CreatePostAsync(request, "user1", "Autor");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El contenido es obligatorio", result.Error);
        await _repo.DidNotReceive().CreatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task CreatePostAsync_CategoriaInvalida_DevuelveError()
    {
        var result = await _service.CreatePostAsync(ValidRequest("Invalida"), "user1", "Autor");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("La categoría no es válida", result.Error);
        await _repo.DidNotReceive().CreatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task GetPostsAsync_CategoriaValida_DevuelvePosts()
    {
        _repo.GetPostsAsync("Tecnologia").Returns(new List<BlogPost> { SamplePost() });

        var result = await _service.GetPostsAsync("Tecnologia");

        Assert.True(result.StatusCode == 200);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetPostsAsync_CategoriaInvalida_DevuelveError()
    {
        var result = await _service.GetPostsAsync("CategoriaQueNoExiste");

        Assert.Equal(400, result.StatusCode);
        await _repo.DidNotReceive().GetPostsAsync(Arg.Any<string?>());
    }

    [Fact]
    public async Task GetPostAsync_NoExiste_DevuelveNotFound()
    {
        _repo.GetPostByIdAsync("desconocido").Returns((BlogPost?)null);

        var result = await _service.GetPostAsync("desconocido");

        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetPostAsync_Existe_DevuelvePost()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost());

        var result = await _service.GetPostAsync("post1");

        Assert.Equal(200, result.StatusCode);
        Assert.Equal("post1", result.Value!.Id);
    }

    [Fact]
    public async Task UpdatePostAsync_PostNoExiste_DevuelveNotFound()
    {
        _repo.GetPostByIdAsync("post1").Returns((BlogPost?)null);

        var result = await _service.UpdatePostAsync("post1", ValidRequest(), "user1", false);

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().UpdatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task UpdatePostAsync_NoEsElAutor_NiAdmin_DevuelveForbidden()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost(userId: "otro"));

        var result = await _service.UpdatePostAsync("post1", ValidRequest(), "user1", false);

        Assert.Equal(403, result.StatusCode);
        await _repo.DidNotReceive().UpdatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task UpdatePostAsync_EsElAutor_Actualiza()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost());
        _repo.UpdatePostAsync(Arg.Any<BlogPost>()).Returns(true);

        var result = await _service.UpdatePostAsync("post1", ValidRequest(), "user1", false);

        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Mi primer post", result.Value!.Title);
        Assert.True(result.Value.UpdatedAt is not null);
        await _repo.Received(1).UpdatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task UpdatePostAsync_EsAdmin_PuedeActualizarPostDeOtro()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost(userId: "otro"));
        _repo.UpdatePostAsync(Arg.Any<BlogPost>()).Returns(true);

        var result = await _service.UpdatePostAsync("post1", ValidRequest(), "user1", true);

        Assert.Equal(200, result.StatusCode);
        await _repo.Received(1).UpdatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task UpdatePostAsync_PayloadInvalido_DevuelveErrorYNoActualiza()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost());
        var request = ValidRequest();
        request.Content = string.Empty;

        var result = await _service.UpdatePostAsync("post1", request, "user1", false);

        Assert.Equal(400, result.StatusCode);
        await _repo.DidNotReceive().UpdatePostAsync(Arg.Any<BlogPost>());
    }

    [Fact]
    public async Task DeletePostAsync_PostNoExiste_DevuelveNotFound()
    {
        _repo.GetPostByIdAsync("post1").Returns((BlogPost?)null);

        var result = await _service.DeletePostAsync("post1", "user1", false);

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().DeletePostAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeletePostAsync_NoEsElAutor_NiAdmin_DevuelveForbidden()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost(userId: "otro"));

        var result = await _service.DeletePostAsync("post1", "user1", false);

        Assert.Equal(403, result.StatusCode);
        await _repo.DidNotReceive().DeletePostAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task DeletePostAsync_EsElAutor_Elimina()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost());
        _repo.DeletePostAsync("post1").Returns(true);

        var result = await _service.DeletePostAsync("post1", "user1", false);

        Assert.Equal(200, result.StatusCode);
        await _repo.Received(1).DeletePostAsync("post1");
    }

    [Fact]
    public async Task DeletePostAsync_EsAdmin_EliminaPostDeOtro()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost(userId: "otro"));
        _repo.DeletePostAsync("post1").Returns(true);

        var result = await _service.DeletePostAsync("post1", "user1", true);

        Assert.Equal(200, result.StatusCode);
        await _repo.Received(1).DeletePostAsync("post1");
    }

    [Fact]
    public async Task DeletePostAsync_RepoNoElimina_DevuelveNotFound()
    {
        _repo.GetPostByIdAsync("post1").Returns(SamplePost());
        _repo.DeletePostAsync("post1").Returns(false);

        var result = await _service.DeletePostAsync("post1", "user1", false);

        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task AddCommentAsync_PostNoExiste_DevuelveNotFound()
    {
        _repo.PostExistsAsync("post1").Returns(false);

        var result = await _service.AddCommentAsync("post1", new BlogCommentRequest { Text = "Hola" }, "user2", "Otro");

        Assert.Equal(404, result.StatusCode);
        await _repo.DidNotReceive().AddCommentAsync(Arg.Any<BlogComment>());
    }

    [Fact]
    public async Task AddCommentAsync_TextoVacio_DevuelveError()
    {
        _repo.PostExistsAsync("post1").Returns(true);

        var result = await _service.AddCommentAsync("post1", new BlogCommentRequest { Text = "  " }, "user2", "Otro");

        Assert.Equal(400, result.StatusCode);
        Assert.Equal("El comentario no puede estar vacío", result.Error);
        await _repo.DidNotReceive().AddCommentAsync(Arg.Any<BlogComment>());
    }

    [Fact]
    public async Task AddCommentAsync_ComentarioValido_CreaComentario()
    {
        _repo.PostExistsAsync("post1").Returns(true);

        var result = await _service.AddCommentAsync("post1", new BlogCommentRequest { Text = "  Buen post  " }, "user2", "Otro");

        Assert.Equal(201, result.StatusCode);
        Assert.Equal("post1", result.Value!.PostId);
        Assert.Equal("user2", result.Value.UserId);
        Assert.Equal("Otro", result.Value.AuthorName);
        Assert.Equal("Buen post", result.Value.Text);
        await _repo.Received(1).AddCommentAsync(Arg.Any<BlogComment>());
    }

    [Fact]
    public async Task GetCommentsAsync_PostNoExiste_DevuelveNotFound()
    {
        _repo.PostExistsAsync("post1").Returns(false);

        var result = await _service.GetCommentsAsync("post1");

        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetCommentsAsync_DevuelveComentarios()
    {
        _repo.PostExistsAsync("post1").Returns(true);
        _repo.GetCommentsAsync("post1").Returns(new List<BlogComment>
        {
            new() { Id = "c1", PostId = "post1", AuthorName = "Otro", Text = "Hola", CreatedAt = DateTime.UtcNow }
        });

        var result = await _service.GetCommentsAsync("post1");

        Assert.Equal(200, result.StatusCode);
        Assert.Single(result.Value!);
    }

    [Theory]
    [InlineData("General", true)]
    [InlineData("Tecnologia", true)]
    [InlineData("Personal", true)]
    [InlineData("Tutorial", true)]
    [InlineData("tecnologia", true)]
    [InlineData("Recetas", false)]
    [InlineData("", false)]
    public void IsValidCategory_ValidaCategoriasPermitidas(string category, bool expected) =>
        Assert.Equal(expected, BlogService.IsValidCategory(category));

    [Fact]
    public void AllowedCategories_ContieneLasCategoriasDelBlog()
    {
        Assert.Equal(4, BlogService.AllowedCategories.Length);
        Assert.Contains("General", BlogService.AllowedCategories);
        Assert.Contains("Tecnologia", BlogService.AllowedCategories);
        Assert.Contains("Personal", BlogService.AllowedCategories);
        Assert.Contains("Tutorial", BlogService.AllowedCategories);
    }
}