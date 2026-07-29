using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalReportsController : ControllerBase
{
    private readonly MongoDbContext _db;
    private readonly PdfExportService _pdfExport;

    public MedicalReportsController(MongoDbContext db, PdfExportService pdfExport)
    {
        _db = db;
        _pdfExport = pdfExport;
    }

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

    [HttpGet("export/{id}")]
    public async Task<IActionResult> ExportPdf(string id)
    {
        var report = await _db.MedicalReports.Find(r => r.Id == id).FirstOrDefaultAsync();
        if (report is null)
            return NotFound();

        var pdfBytes = _pdfExport.GenerateReportPdf(report);
        return File(pdfBytes, "application/pdf", $"informe_{id}.pdf");
    }

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
        var filters = new List<FilterDefinition<MedicalReport>> { Builders<MedicalReport>.Filter.Eq(r => r.Id, id) };

        if (ObjectId.TryParse(id, out var objectId))
        {
            filters.Add(Builders<MedicalReport>.Filter.Eq("_id", objectId));
        }

        var result = await _db.MedicalReports.DeleteOneAsync(Builders<MedicalReport>.Filter.Or(filters));
        return result.IsAcknowledged && result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}