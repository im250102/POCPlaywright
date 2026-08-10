using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly MongoDbContext _db;
    private readonly PatientNotificationService _notifications;

    public PatientsController(MongoDbContext db, PatientNotificationService notifications)
    {
        _db = db;
        _notifications = notifications;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Patients.Find(_ => true).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var patient = await _db.Patients.Find(p => p.Id == id).FirstOrDefaultAsync();
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        await _db.Patients.InsertOneAsync(patient);
        _ = _notifications.SendWelcomeMessageAsync(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Patient updated)
    {
        var result = await _db.Patients.ReplaceOneAsync(p => p.Id == id, updated);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var filters = new List<FilterDefinition<Patient>> { Builders<Patient>.Filter.Eq(p => p.Id, id) };

        if (ObjectId.TryParse(id, out var objectId))
        {
            filters.Add(Builders<Patient>.Filter.Eq("_id", objectId));
        }

        var result = await _db.Patients.DeleteOneAsync(Builders<Patient>.Filter.Or(filters));
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
