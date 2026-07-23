using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalReportsController : ControllerBase
{
    private readonly MongoDbContext _db;

    public MedicalReportsController(MongoDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.MedicalReports.Find(_ => true).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var report = await _db.MedicalReports.Find(r => r.Id == id).FirstOrDefaultAsync();
        return report is null ? NotFound() : Ok(report);
    }

    [HttpGet("by-patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(string patientId) =>
        Ok(await _db.MedicalReports.Find(r => r.PatientId == patientId).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MedicalReport report)
    {
        await _db.MedicalReports.InsertOneAsync(report);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] MedicalReport updated)
    {
        var result = await _db.MedicalReports.ReplaceOneAsync(r => r.Id == id, updated);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _db.MedicalReports.DeleteOneAsync(r => r.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
