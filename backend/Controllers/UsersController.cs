using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly MongoDbContext _db;

    public UsersController(MongoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAdmin()) return Forbid();

        var users = await _db.Users.Find(_ => true).SortByDescending(u => u.CreatedAt).ToListAsync();
        var result = users.Select(u => new UserDto
        {
            Id = u.Id ?? string.Empty,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        });

        return Ok(result);
    }

    [HttpGet("{id}/accesses")]
    public async Task<IActionResult> GetAccesses(string id)
    {
        if (!IsAdmin()) return Forbid();

        var accesses = await _db.UserAccesses
            .Find(a => a.UserId == id)
            .SortByDescending(a => a.LoginAt)
            .ToListAsync();

        var result = accesses.Select(a => new UserAccessDto
        {
            Id = a.Id ?? string.Empty,
            UserId = a.UserId,
            LoginAt = a.LoginAt
        });

        return Ok(result);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
    {
        if (!IsAdmin()) return Forbid();

        if (request.Role != "Admin" && request.Role != "Medico")
            return BadRequest(new { error = "El rol debe ser 'Admin' o 'Medico'" });

        var result = await _db.Users.UpdateOneAsync(
            u => u.Id == id,
            Builders<User>.Update.Set(u => u.Role, request.Role));

        return result.IsAcknowledged && result.MatchedCount > 0 ? NoContent() : NotFound();
    }

    private bool IsAdmin()
    {
        var role = User.FindFirst("role")?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return role == "Admin";
    }
}
