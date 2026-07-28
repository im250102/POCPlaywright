using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MongoDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(MongoDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Todos los campos son obligatorios" });

        if (request.Password != request.ConfirmPassword)
            return BadRequest(new { error = "Las contraseñas no coinciden" });

        if (request.Password.Length < 8)
            return BadRequest(new { error = "La contraseña debe tener al menos 8 caracteres" });

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _db.Users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
        if (existing is not null)
            return BadRequest(new { error = "El email ya está registrado" });

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _db.Users.InsertOneAsync(user);

        var token = GenerateToken(user);

        return CreatedAtAction(null, new AuthResponse
        {
            Id = user.Id ?? string.Empty,
            Name = user.Name,
            Email = user.Email,
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email y contraseña son obligatorios" });

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
        if (user is null)
            return Unauthorized(new { error = "Credenciales inválidas" });

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Credenciales inválidas" });

        var token = GenerateToken(user);

        return Ok(new AuthResponse
        {
            Id = user.Id ?? string.Empty,
            Name = user.Name,
            Email = user.Email,
            Token = token
        });
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? "POCPlaywright_SuperSecretKey_2026_MinLength32Chars!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenLifetimeMinutes = _config.GetValue("Jwt:TokenExpiryMinutes", 60);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "POCPlaywright",
            audience: _config["Jwt:Audience"] ?? "POCPlaywright",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
