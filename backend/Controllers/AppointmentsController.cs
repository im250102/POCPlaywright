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
public class AppointmentsController : ControllerBase
{
    private readonly MongoDbContext _db;

    public AppointmentsController(MongoDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Appointments.Find(a => a.UserId == UserId).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var appointment = await _db.Appointments.Find(a => a.Id == id && a.UserId == UserId).FirstOrDefaultAsync();
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("by-patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(string patientId) =>
        Ok(await _db.Appointments.Find(a => a.PatientId == patientId && a.UserId == UserId).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Appointment appointment)
    {
        appointment.UserId = UserId;
        await _db.Appointments.InsertOneAsync(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Appointment updated)
    {
        updated.UserId = UserId;
        var result = await _db.Appointments.ReplaceOneAsync(a => a.Id == id && a.UserId == UserId, updated);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var filters = new List<FilterDefinition<Appointment>>
        {
            Builders<Appointment>.Filter.Eq(a => a.Id, id),
            Builders<Appointment>.Filter.Eq(a => a.UserId, UserId)
        };

        if (ObjectId.TryParse(id, out var objectId))
        {
            filters.Add(Builders<Appointment>.Filter.Eq("_id", objectId));
        }

        var result = await _db.Appointments.DeleteOneAsync(Builders<Appointment>.Filter.And(filters));
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
