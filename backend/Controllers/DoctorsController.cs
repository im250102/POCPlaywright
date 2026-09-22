using System.Security.Claims;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly DoctorService _doctors;

    public DoctorsController(DoctorService doctors)
    {
        _doctors = doctors;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _doctors.GetDoctorsAsync();
        return Map(result, () => Ok(result.Value!.Select(ToDto)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _doctors.GetDoctorAsync(id);
        return Map(result, () => Ok(ToDto(result.Value!)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DoctorRequest request)
    {
        var result = await _doctors.CreateDoctorAsync(request, GetUserId());
        return Map(result, () => CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            ToDto(result.Value!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] DoctorRequest request)
    {
        var result = await _doctors.UpdateDoctorAsync(id, request);
        return Map(result, () => Ok(ToDto(result.Value!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _doctors.DeleteDoctorAsync(id);
        return Map(result, () => NoContent());
    }

    private static DoctorDto ToDto(Doctor doctor) => new()
    {
        Id = doctor.Id ?? string.Empty,
        UserId = doctor.UserId,
        Name = doctor.Name,
        LastName = doctor.LastName,
        Phone = doctor.Phone,
        Specialty = doctor.Specialty
    };

    private IActionResult Map(DoctorResult result, Func<IActionResult> onSuccess)
    {
        return result.StatusCode switch
        {
            400 => BadRequest(new { error = result.Error }),
            404 => NotFound(),
            _ => onSuccess()
        };
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
}