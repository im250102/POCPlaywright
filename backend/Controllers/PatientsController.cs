using System.Security.Claims;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly MongoDbContext _db;

    public PatientsController(MongoDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Patients.Find(p => p.UserId == UserId).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var patient = await _db.Patients.Find(p => p.Id == id && p.UserId == UserId).FirstOrDefaultAsync();
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        patient.UserId = UserId;
        await _db.Patients.InsertOneAsync(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Patient updated)
    {
        updated.UserId = UserId;
        var result = await _db.Patients.ReplaceOneAsync(p => p.Id == id && p.UserId == UserId, updated);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var filters = new List<FilterDefinition<Patient>>
        {
            Builders<Patient>.Filter.Eq(p => p.Id, id),
            Builders<Patient>.Filter.Eq(p => p.UserId, UserId)
        };

        if (ObjectId.TryParse(id, out var objectId))
        {
            filters.Add(Builders<Patient>.Filter.Eq("_id", objectId));
        }

        var result = await _db.Patients.DeleteOneAsync(Builders<Patient>.Filter.And(filters));
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
