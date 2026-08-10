using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly MongoDbContext _db;

    public NotificationsController(MongoDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.NotificationLogs.Find(_ => true).SortByDescending(n => n.CreatedAt).ToListAsync());

    [HttpGet("by-patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(string patientId) =>
        Ok(await _db.NotificationLogs
            .Find(n => n.PatientId == patientId)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync());
}
