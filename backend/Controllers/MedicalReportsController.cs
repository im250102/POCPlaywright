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
public class MedicalReportsController : ControllerBase
{
    private readonly MongoDbContext _db;

    public MedicalReportsController(MongoDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.MedicalReports.Find(r => r.UserId == UserId).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var report = await _db.MedicalReports.Find(r => r.Id == id && r.UserId == UserId).FirstOrDefaultAsync();
        return report is null ? NotFound() : Ok(report);
    }

    [HttpGet("by-patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(string patientId) =>
        Ok(await _db.MedicalReports.Find(r => r.PatientId == patientId && r.UserId == UserId).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MedicalReport report)
    {
        report.UserId = UserId;
        await _db.MedicalReports.InsertOneAsync(report);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] MedicalReport updated)
    {
        updated.UserId = UserId;
        var result = await _db.MedicalReports.ReplaceOneAsync(r => r.Id == id && r.UserId == UserId, updated);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var filters = new List<FilterDefinition<MedicalReport>>
        {
            Builders<MedicalReport>.Filter.Eq(r => r.Id, id),
            Builders<MedicalReport>.Filter.Eq(r => r.UserId, UserId)
        };

        if (ObjectId.TryParse(id, out var objectId))
        {
            filters.Add(Builders<MedicalReport>.Filter.Eq("_id", objectId));
        }

        var result = await _db.MedicalReports.DeleteOneAsync(Builders<MedicalReport>.Filter.And(filters));
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
