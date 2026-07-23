using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly MongoDbContext _db;

    public AppointmentsController(MongoDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Appointments.Find(_ => true).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var appointment = await _db.Appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("by-patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(string patientId) =>
        Ok(await _db.Appointments.Find(a => a.PatientId == patientId).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Appointment appointment)
    {
        await _db.Appointments.InsertOneAsync(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Appointment updated)
    {
        var result = await _db.Appointments.ReplaceOneAsync(a => a.Id == id, updated);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _db.Appointments.DeleteOneAsync(a => a.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
